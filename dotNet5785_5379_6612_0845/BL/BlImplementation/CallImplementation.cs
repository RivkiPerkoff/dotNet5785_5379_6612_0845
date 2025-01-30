using System.Linq;
using System.Net;
using BL.BIApi;
using BL.BO;

namespace BL.BlImplementation;

internal class CallImplementation : BIApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddCall(Call callObject)
    {
        if (callObject == null)
            throw new ArgumentNullException(nameof(callObject));

        // Validate fields
        if (string.IsNullOrWhiteSpace(callObject.AddressOfCall))
            throw new InvalidOperationException("Address cannot be empty.");
        if (callObject.MaxFinishTime <= callObject.OpeningTime)
            throw new InvalidOperationException("Max finish time must be greater than opening time.");

        // Generate DO object
        var callDO = new Dal.DO.Call
        {
            Id = callObject.IdCall,
            CallType = (Dal.BO.CallTypes)callObject.CallType,
            Description = callObject.CallDescription,
            Address = callObject.AddressOfCall,
            Longitude = callObject.CallLongitude,
            Latitude = callObject.CallLatitude,
            OpeningTime = callObject.OpeningTime,
            MaxFinishTime = callObject.MaxFinishTime,
            Status = (Dal.DO.StatusCallType)callObject.StatusCallType
        };

        // Add call to data layer
        _dal.Call.Add(callDO);
    }

    public void AssignVolunteerToCall(int volunteerId, int callId)
    {
        var call = _dal.Call.Get(callId)
                  ?? throw new InvalidOperationException("Call not found.");

        if (call.Status != Dal.DO.StatusCallType.Open)
            throw new InvalidOperationException("Call is not open for assignment.");

        var assignment = new Dal.DO.Assignment
        {
            CallId = callId,
            VolunteerId = volunteerId,
            StartTime = DateTime.Now,
            EndTime = null
        };

        _dal.Assignment.Add(assignment);
    }

    public void CancelCallTreatment(int requesterId, int assignmentId)
    {
        var assignment = _dal.Assignment.Get(assignmentId)
                         ?? throw new InvalidOperationException("Assignment not found.");

        if (assignment.EndTime != null)
            throw new InvalidOperationException("Cannot cancel a completed treatment.");

        var requesterIsManager = _dal.Volunteer.IsManager(requesterId);
        if (assignment.VolunteerId != requesterId && !requesterIsManager)
            throw new InvalidOperationException("Unauthorized cancellation.");

        assignment.EndTime = DateTime.Now;
        assignment.Status = assignment.VolunteerId == requesterId
            ? Dal.DO.AssignmentStatus.SelfCancelled
            : Dal.DO.AssignmentStatus.ManagerCancelled;

        _dal.Assignment.Update(assignment);
    }

    public void CompleteCallTreatment(int volunteerId, int assignmentId)
    {
        var assignment = _dal.Assignment.Get(assignmentId)
                         ?? throw new InvalidOperationException("Assignment not found.");

        if (assignment.VolunteerId != volunteerId)
            throw new InvalidOperationException("Unauthorized completion.");

        if (assignment.EndTime != null)
            throw new InvalidOperationException("Treatment is already completed.");

        assignment.EndTime = DateTime.Now;
        assignment.Status = Dal.DO.AssignmentStatus.Completed;

        _dal.Assignment.Update(assignment);
    }

    public void DeleteCall(int callId)
    {
        var call = _dal.Call.Get(callId)
                  ?? throw new InvalidOperationException("Call not found.");

        if (call.Status != Dal.DO.StatusCallType.Open || _dal.Assignment.ExistsForCall(callId))
            throw new InvalidOperationException("Cannot delete this call.");

        _dal.Call.Delete(callId);
    }

    public int[] GetCallAmountsByStatus()
    {
        var calls = _dal.Call.GetAll();
        return calls
            .GroupBy(call => (int)call.Status)
            .OrderBy(group => group.Key)
            .Select(group => group.Count())
            .ToArray();
    }

    public IEnumerable<ClosedCallInList> GetClosedCallsForVolunteer(int volunteerId, CallTypes? callType, Enum? sortField)
    {
        var calls = _dal.Call.GetAll()
            .Where(call => call.Status == Dal.DO.StatusCallType.Closed && call.AssignedVolunteerId == volunteerId);

        if (callType.HasValue)
            calls = calls.Where(call => call.CallType == (Dal.DO.CallTypes)callType.Value);

        return SortCalls(calls, sortField).Select(ToClosedCallInList);
    }

    public IEnumerable<CallInList> GetFilteredAndSortedCallList(Enum? filterField, object? filterValue, Enum? sortField)
    {
        var calls = _dal.Call.GetAll();

        if (filterField != null && filterValue != null)
            calls = calls.Where(call => GetFieldValue(call, filterField).Equals(filterValue));

        return SortCalls(calls, sortField).Select(ToCallInList);
    }

    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteerSelection(int volunteerId, Enum? filterField, Enum? sortField)
    {
        var calls = _dal.Call.GetAll()
            .Where(call => call.Status == Dal.DO.StatusCallType.Open || call.Status == Dal.DO.StatusCallType.OpenWithRisk);

        if (filterField != null)
            calls = calls.Where(call => GetFieldValue(call, filterField).Equals(filterField));

        return SortCalls(calls, sortField).Select(ToOpenCallInList);
    }

    public void UpdateCallDetails(Call callObject)
    {
        if (callObject == null)
            throw new ArgumentNullException(nameof(callObject));

        if (string.IsNullOrWhiteSpace(callObject.AddressOfCall))
            throw new InvalidOperationException("Invalid address.");

        if (callObject.MaxFinishTime <= callObject.OpeningTime)
            throw new InvalidOperationException("Invalid time range.");

        var call = _dal.Call.Get(callObject.IdCall)
                  ?? throw new InvalidOperationException("Call not found.");

        call.Address = callObject.AddressOfCall;
        call.Latitude = callObject.CallLatitude;
        call.Longitude = callObject.CallLongitude;
        call.MaxFinishTime = callObject.MaxFinishTime;
        call.Status = (Dal.DO.StatusCallType)callObject.StatusCallType;

        _dal.Call.Update(call);
    }

    // Helper functions
    private IEnumerable<Dal.DO.Call> SortCalls(IEnumerable<Dal.DO.Call> calls, Enum? sortField)
    {
        return sortField switch
        {
            CallSortField.Number => calls.OrderBy(call => call.Id),
            CallSortField.Priority => calls.OrderBy(call => call.Priority),
            _ => calls.OrderBy(call => call.Id)
        };
    }

    private object GetFieldValue(Dal.DO.Call call, Enum? field)
    {
        // Reflection or a dictionary mapping can be used to get the value based on the field
        throw new NotImplementedException();
    }

    private CallInList ToCallInList(Dal.DO.Call call)
    {
        return new CallInList
        {
            Id = call.Id,
            Address = call.Address,
            Status = call.Status
        };
    }

    private ClosedCallInList ToClosedCallInList(Dal.DO.Call call)
    {
        return new ClosedCallInList
        {
            Id = call.Id,
            Address = call.Address,
            Status = call.Status
        };
    }

    private OpenCallInList ToOpenCallInList(Dal.DO.Call call)
    {
        return new OpenCallInList
        {
            Id = call.Id,
            Address = call.Address,
            Status = call.Status
        };
    }
}
