using System.Linq;
using System.Net;
using BL.BIApi;
using BL.BO;
using BL.Helpers;

namespace BL.BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddCall(BL.BO.Call callObject)
    {
        if (callObject == null)
            throw new ArgumentNullException(nameof(callObject));

        // Validate fields
        if (string.IsNullOrWhiteSpace(callObject.AddressOfCall))
            throw new InvalidOperationException("Address cannot be empty.");
        if (callObject.MaxFinishTime <= callObject.OpeningTime)
            throw new InvalidOperationException("Max finish time must be greater than opening time.");

        // Generate DO object
        var callDO = new DO.Call
        (
            idCall: callObject.IdCall,
            callDescription: callObject.CallDescription,
            callAddress: callObject.AddressOfCall,
            callLatitude: callObject.CallLatitude,
            callLongitude: callObject.CallLongitude,
            openingTime: callObject.OpeningTime,
            maxFinishTime: callObject.MaxFinishTime ?? default,
            CallTypes: (DO.CallTypes)callObject.CallType
        );



        // Add call to data layer
        _dal.Call.Create(callDO);
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
        var assignment = _dal.Assignment.Read(assignmentId)
                         ?? throw new InvalidOperationException("Assignment not found.");

        if (assignment.EndTimeForTreatment != null)
            throw new InvalidOperationException("Cannot cancel a completed treatment.");

        var requester = _dal.Volunteer.Read(requesterId)
                      ?? throw new InvalidOperationException("Volunteer not found.");

        if ((assignment.VolunteerId != requesterId) && (requester.Role != DO.Role.Manager))
            throw new InvalidOperationException("Unauthorized cancellation.");

        // יצירת עותק חדש עם השדות המעודכנים
        assignment = assignment with
        {
            EndTimeForTreatment = ClockManager.Now,
            FinishCallType = (assignment.VolunteerId == requesterId)
                ? DO.FinishCallType.CanceledByVolunteer
                : DO.FinishCallType.CanceledByManager
        };

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


    public void UpdateCallDetails(BL.BO.Call callObject)
    {
        if (callObject == null)
            throw new ArgumentNullException(nameof(callObject));

        // ניסיון להשיג קואורדינטות מהכתובת
        try
        {
            (double latitude, double longitude) = Helpers.Tools.GetCoordinatesFromAddress(callObject.AddressOfCall);
            callObject.CallLatitude = latitude;
            callObject.CallLongitude = longitude;
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Invalid address: Unable to retrieve coordinates.");
        }

        // בדיקה לוגית: יש לוודא ש־MaxFinishTime קיים ושזמן סיום גדול מזמן הפתיחה
        if (!callObject.MaxFinishTime.HasValue || callObject.MaxFinishTime.Value <= callObject.OpeningTime)
            throw new InvalidOperationException("Invalid time range: Max finish time must be after opening time.");

        // שליפת הקריאה מה-DAL (שכבת הנתונים)
        var existingCall = _dal.Call.Read(callObject.IdCall)
                            ?? throw new InvalidOperationException("Call not found.");

        // יצירת אובייקט DO חדש עם הערכים המעודכנים באמצעות with-expression
        var updatedCall = existingCall with
        {
            CallAddress = callObject.AddressOfCall,
            CallLatitude = callObject.CallLatitude,
            CallLongitude = callObject.CallLongitude,
            MaxFinishTime = callObject.MaxFinishTime,
            // המרת סוג הקריאה מ-BO ל-DO. יש לוודא ששני ה-enum תואמים:
            CallTypes = (DO.CallTypes)callObject.CallType
        };

        // ביצוע העדכון בשכבת הנתונים
        _dal.Call.Update(updatedCall);
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

    public BO.Call GetCallDetails(int callId)
    {
        // קבלת הקריאה מה-DAL
        var call = _dal.Call.Read(callId)
                  ?? throw new InvalidOperationException("Call not found.");

        // קבלת כל ההקצאות המשויכות לקריאה
        var assignments = _dal.Assignment.ReadAll()
            .Where(a => a.IdOfRunnerCall == callId)
            .Select(a => new BO.CallAssignInList
            {
                VolunteerId = a.VolunteerId,
                VolunteerName = _dal.Volunteer.Read(a.VolunteerId)?.Name, // שליפת שם המתנדב
                EntryTimeForTreatment = a.EntryTimeForTreatment,
                EndTimeForTreatment = a.EndTimeForTreatment,
                TreatmentEndType = (BO.TreatmentEndType?)(a.FinishCallType) // המרה נכונה לסוג סיום טיפול
            })
            .ToList();


        // יצירת אובייקט הלוגיקה העסקית (BO) של הקריאה והוספת ההקצאות
        return new BO.Call
        {
            IdCall = call.IdCall,
            CallType = (BO.CallTypes)call.CallTypes,
            CallDescription = call.CallDescription,
            AddressOfCall = call.CallAddress,
            CallLatitude = call.CallLatitude,
            OpeningTime = call.OpeningTime,
            MaxFinishTime = call.MaxFinishTime,
            StatusCallType = (BO.StatusCallType)call.CallTypes,
            CallAssignInLists = assignments
        };
    }

}
