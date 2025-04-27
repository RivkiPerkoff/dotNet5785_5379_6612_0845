using BL.BIApi;
using BL.BO;
using BL.Helpers;
using DalApi;
using DO;
using Helpers;
using NSubstitute.Core;
using System.Net;

namespace BL.BlImplementation;

internal class CallImplementation : BIApi.ICall
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Returns the count of calls grouped by their status (open, in progress, etc.).
    /// </summary>
    /// <returns>An array of integers representing the count of calls for each status.</returns>
    public int[] GetCallAmounts()

    {
        IEnumerable<DO.Call> doCalls = _dal.Call.ReadAll();
        IEnumerable<BO.Call> boCalls = CallManager.ConvertToBOCalls(doCalls);

        int enumSize = Enum.GetValues(typeof(StatusCallType)).Length;
        int[] statusCounts = new int[enumSize];

        foreach (var group in boCalls.GroupBy(call => (int)call.StatusCallType))
        {
            statusCounts[group.Key] = group.Count();
        }

        return statusCounts;
    }

    /// <summary>
    /// Retrieves the detailed information of a specific call by its ID.
    /// </summary>
    /// <param name="CallId">The ID of the call to retrieve.</param>
    /// <returns>A BO.Call object containing the details of the specified call.</returns>
    /// <exception cref="Exception">Thrown if the call with the given ID is not found.</exception>
    public BO.Call GetCallDetails(int CallId)

    {
        var call = _dal.Call.Read(CallId);

        if (call == null)
            throw new Exception($"Call with ID {CallId} not found.");

        return new BO.Call
        {
            IdCall = call.IdCall,
            CallType = (BO.CallTypes)call.CallTypes,
            CallDescription = call.CallDescription,
            AddressOfCall = call.CallAddress,
            CallLongitude = call.CallLongitude,
            CallLatitude = call.CallLatitude,
            OpeningTime = (DateTime)call.OpeningTime,
            MaxFinishTime = call.MaxFinishTime,
            CallAssignInLists = null,
            StatusCallType = Tools.GetCallStatus(call)
        };
    }
    /// <summary>
    /// Deletes a specific call by its ID, only if the call is open and has no assignments.
    /// </summary>
    /// <param name="callId">The ID of the call to delete.</param>
    /// <exception cref="BlGeneralDatabaseException">Thrown if the call cannot be deleted.</exception>
    public void DeleteCall(int callId)
    {
        try
        {
            var call = _dal.Call.Read(callId) ?? throw new DO.DalDoesNotExistException($"Call with ID {callId} not found.");
            var callStatus = Tools.GetCallStatus(call);
            var assignments = _dal.Assignment.ReadAll(a => a.IdOfRunnerCall == callId);
            if (callStatus != StatusCallType.open || assignments.Any())
                throw new BlGeneralDatabaseException("Cannot delete this call. Only open calls that have never been assigned can be deleted.");
            _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated(); //stage 5

        }
        catch (DO.DalDeletionImpossible ex)
        {
            throw new BO.BlDoesNotExistException("Call not found in data layer.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while geting Volunteer details.", ex);
        }
    }
    /// <summary>
    /// Adds a new call to the system with the provided details.
    /// </summary>
    /// <param name="callObject">A BO.Call object containing the new call's information.</param>
    /// <exception cref="ArgumentNullException">Thrown if the callObject is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the address is empty or the max finish time is not valid.</exception>
    public void AddCall(BL.BO.Call callObject)

    {
        if (callObject == null)
            throw new ArgumentNullException(nameof(callObject));
        if (string.IsNullOrWhiteSpace(callObject.AddressOfCall))
            throw new InvalidOperationException("Address cannot be empty.");
        if (callObject.MaxFinishTime <= callObject.OpeningTime)
            throw new InvalidOperationException("Max finish time must be greater than opening time.");
        var coordinates = Tools.GetCoordinatesFromAddress(callObject.AddressOfCall);

        var callDO = new DO.Call
        (
            idCall: callObject.IdCall,
            callDescription: callObject.CallDescription,
            callAddress: callObject.AddressOfCall,
            callLatitude: coordinates.Item1,
            callLongitude: coordinates.Item2,
            openingTime: callObject.OpeningTime,
            maxFinishTime: callObject.MaxFinishTime ?? default,
            CallTypes: (DO.CallTypes)callObject.CallType
        );
        _dal.Call.Create(callDO);
        CallManager.Observers.NotifyListUpdated(); //stage 5

    }
    /// <summary>
    /// Retrieves a list of closed calls assigned to a specific volunteer, optionally filtered and sorted.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="filterType">An optional filter for the type of calls to return.</param>
    /// <param name="sortField">An optional field to sort the returned calls by.</param>
    /// <returns>A list of BO.ClosedCallInList objects representing the closed calls for the volunteer.</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsForVolunteer(int volunteerId, BO.CallTypes? filterType = null, BO.ClosedCallInListFields? sortField = null)
    {
        try
        {
            var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.EndTimeForTreatment != null);  // Only closed calls

            var callIds = assignments.Select(a => a.IdOfRunnerCall).Distinct();
            var calls = _dal.Call.ReadAll(c => callIds.Contains(c.IdCall));

            var closedCalls = CallManager.CreateClosedCallList(calls, assignments);
            if (filterType.HasValue)
            {
                closedCalls = closedCalls.Where(c => (BO.CallTypes)c.CallTypes == filterType.Value);
            }


            return sortField switch
            {
                BO.ClosedCallInListFields.CallTypes => closedCalls.OrderBy(c => c.CallTypes),
                BO.ClosedCallInListFields.Address => closedCalls.OrderBy(c => c.Address),
                BO.ClosedCallInListFields.OpeningTime => closedCalls.OrderBy(c => c.OpeningTime),
                BO.ClosedCallInListFields.EntryTimeForTreatment => closedCalls.OrderBy(c => c.EntryTimeForTreatment),
                BO.ClosedCallInListFields.EndTimeForTreatment => closedCalls.OrderBy(c => c.EntryTimeForTreatment),
                BO.ClosedCallInListFields.FinishCallType => closedCalls.OrderBy(c => c.FinishCallType),
                _ => closedCalls.OrderBy(c => c.Id)
            };
        }

        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Could not find data for volunteer {volunteerId}", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException("An error occurred while retrieving closed calls", ex);
        }
    }
    /// <summary>
    /// Retrieves a list of open calls assigned to a specific volunteer, optionally filtered and sorted.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="filterField">An optional filter for the type of calls to return.</param>
    /// <param name="sortField">An optional field to sort the returned calls by.</param>
    /// <returns>A list of OpenCallInList objects representing the open calls for the volunteer.</returns>
    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteerSelection(int volunteerId, BO.CallTypes? filterField, OpenCallInListFields? sortField)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");

            var volunteerAssignments = _dal.Assignment.ReadAll()
                .Where(a => a.VolunteerId == volunteerId) 
                .Select(a => a.IdOfRunnerCall); 
            var openCalls = _dal.Call.ReadAll()
                .Where(c => volunteerAssignments.Contains(c.IdCall) &&
                            (Tools.GetCallStatus(c) == StatusCallType.open || Tools.GetCallStatus(c) == StatusCallType.openInRisk)) 
                .Join(_dal.Assignment.ReadAll(), 
                      call => call.IdCall, 
                      assignment => assignment.IdOfRunnerCall,
                      (call, assignment) => new OpenCallInList 
                      {
                          Id = call.IdCall, 
                          CallTypes = call.CallTypes, 
                          CallDescription = call.CallDescription, 
                          Address = call.CallAddress, 
                          OpeningTime = (DateTime)call.OpeningTime,
                          MaxFinishTime = call.MaxFinishTime,
                          CallDistance = Tools.DistanceCalculation(volunteer.AddressVolunteer, call.CallAddress)
                      });
            if (filterField.HasValue)
                openCalls = openCalls.Where(c =>
                {
                    return (BO.CallTypes)c.CallTypes == filterField.Value;
                }); 

            openCalls = sortField.HasValue
                ? openCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString())?.GetValue(c)) 
                : openCalls.OrderBy(c => c.Id); 

            return openCalls.ToList(); 
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the open calls list.", ex);
        }
    }
    /// <summary>
    /// Marks a call treatment as complete for a specific volunteer and assignment.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer completing the treatment.</param>
    /// <param name="assignmentId">The ID of the assignment being completed.</param>
    /// <exception cref="BlPermissionException">Thrown if the volunteer does not have permission to complete the assignment.</exception>
    /// <exception cref="BlInvalidOperationException">Thrown if the assignment has already been completed or expired.</exception>
    public void CompleteCallTreatment(int volunteerId, int assignmentId)
    {
        try
        {
            var assignment = _dal.Assignment.Read(assignmentId)
                ?? throw new DO.DalDoesNotExistException($"Assignment with ID={assignmentId} does not exist.");

            if (assignment.VolunteerId != volunteerId)
                throw new BO.BlPermissionException("Unauthorized: The volunteer is not assigned to this task.");

            if (assignment.FinishCallType == FinishCallType.None)
            {
                var updatedAssignment = assignment with
                {
                    FinishCallType = FinishCallType.TakenCareof,
                    EndTimeForTreatment = AdminManager.Now
                };
                _dal.Assignment.Update(updatedAssignment);
                CallManager.Observers.NotifyItemUpdated(updatedAssignment.IdOfRunnerCall); //stage 5
                CallManager.Observers.NotifyListUpdated();
            }
            else throw new BO.BlInvalidOperationException("Assignment has already been completed or expired or canceled.");

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlGeneralDatabaseException("The specified assignment does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while completing the call treatment.", ex);
        }
    }
    /// <summary>
    /// Assigns a call to a volunteer for treatment.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer being assigned the call.</param>
    /// <param name="callId">The ID of the call to assign.</param>
    /// <exception cref="BlInvalidOperationException">Thrown if the call is already assigned or expired.</exception>
    public void ChoosingCallForTreatment(int volunteerId, int callId)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId)
                ?? throw new BO.BlGeneralDatabaseException($"Volunteer with ID={volunteerId} does not exist.");

            var call = _dal.Call.Read(callId)
                ?? throw new BO.BlGeneralDatabaseException($"Call with ID={callId} does not exist.");

            var callStatus = Tools.GetCallStatus(call);
            if (callStatus != StatusCallType.open && callStatus != StatusCallType.openInRisk)
                throw new BO.BlInvalidOperationException("Call is already assigned or has expired.");

            var existingAssignment = _dal.Assignment.ReadAll()
                .FirstOrDefault(a => a.IdOfRunnerCall == callId && a.EndTimeForTreatment == null);
            var newAssignmentId = _dal.Config.CreateAssignmentId();
            if (existingAssignment != null)
                throw new BO.BlInvalidOperationException("Call is already being handled by another volunteer.");
            var newAssignment= new DO.Assignment(newAssignmentId, callId, volunteerId,  DO.FinishCallType.None, AdminManager.Now,null);
         
            _dal.Assignment.Create(newAssignment); 
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlGeneralDatabaseException("Database error while accessing call or volunteer data.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while assigning the call.", ex);
        }
    }
    /// <summary>
    /// Cancels a call treatment for a specific assignment.
    /// </summary>
    /// <param name="requesterId">The ID of the volunteer or manager requesting the cancellation.</param>
    /// <param name="assignmentId">The ID of the assignment to cancel.</param>
    /// <exception cref="InvalidOperationException">Thrown if the assignment has already been completed or if the requester is unauthorized.</exception>
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

        assignment = assignment with
        {
            EndTimeForTreatment = AdminManager.Now,
            FinishCallType = (assignment.VolunteerId == requesterId)
                ? DO.FinishCallType.CanceledByVolunteer
                : DO.FinishCallType.CanceledByManager
        };

        _dal.Assignment.Update(assignment);
        CallManager.Observers.NotifyItemUpdated(assignment.IdOfRunnerCall); //stage 5
        CallManager.Observers.NotifyListUpdated();
    }

    /// <summary>
    /// Updates the details of an existing call.
    /// </summary>
    /// <param name="callObject">A BO.Call object containing the updated call information.</param>
    /// <exception cref="ArgumentNullException">Thrown if the callObject is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the call is not found or the update is invalid.</exception>
    public void UpdateCallDetails(BL.BO.Call callObject)
    {
        if (callObject == null)
            throw new ArgumentNullException(nameof(callObject));

        try
        {

            var (latitude, longitude) = Helpers.Tools.GetCoordinatesFromAddress(callObject.AddressOfCall);
            callObject.CallLatitude = latitude;
            callObject.CallLongitude = longitude;
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Invalid address: Unable to retrieve coordinates.");
        }

        if (!callObject.MaxFinishTime.HasValue || callObject.MaxFinishTime.Value <= callObject.OpeningTime)
            throw new InvalidOperationException("Invalid time range: Max finish time must be after opening time.");

        var existingCall = _dal.Call.Read(callObject.IdCall)
                            ?? throw new InvalidOperationException("Call not found.");

        var updatedCall = existingCall with
        {
            CallAddress = callObject.AddressOfCall,
            CallLatitude = callObject.CallLatitude,
            CallLongitude = callObject.CallLongitude,
            MaxFinishTime = callObject.MaxFinishTime,
            CallTypes = (DO.CallTypes)callObject.CallType
        };

        _dal.Call.Update(updatedCall);
        CallManager.Observers.NotifyItemUpdated(updatedCall.IdCall); //stage 5
        CallManager.Observers.NotifyListUpdated(); //stage 5


    }
    /// <summary>
    /// Retrieves a filtered and sorted list of calls based on provided parameters.
    /// </summary>
    /// <param name="filterBy">An optional field to filter the calls by.</param>
    /// <param name="filterValue">The value to filter by, corresponding to the filterBy parameter.</param>
    /// <param name="sortBy">An optional field to sort the calls by.</param>
    /// <returns>A list of BO.CallInList objects representing the filtered and sorted calls.</returns>
    public IEnumerable<CallInList> GetFilteredAndSortedCallList(CallInListFields? filterBy = null, object? filterValue = null, CallInListFields? sortBy = null)
    {

        IEnumerable<DO.Call> allCalls = _dal.Call.ReadAll().ToList();
        IEnumerable<DO.Assignment> allAssignments = _dal.Assignment.ReadAll().ToList();

        IEnumerable<BO.CallInList> callList = allCalls.Select(call =>
        {
            var latestAssignment = allAssignments
                .Where(a => a.IdOfRunnerCall == call.IdCall)
                .OrderByDescending(a => a.EntryTimeForTreatment)
                .FirstOrDefault();

            return new BO.CallInList
            {
                Id = latestAssignment?.NextAssignmentId,
                CallId = call.IdCall,
                CallType = (BO.CallTypes)call.CallTypes,
                StartTime = (DateTime)call.OpeningTime,
                TimeToEnd = call.MaxFinishTime.HasValue ? (call.MaxFinishTime.Value > DateTime.Now ? call.MaxFinishTime.Value - DateTime.Now : TimeSpan.Zero) : null,
                LastUpdateBy = latestAssignment != null ? _dal.Volunteer.Read(latestAssignment.VolunteerId)?.Name : null,
                TimeTocompleteTreatment = latestAssignment?.EndTimeForTreatment.HasValue == true ? latestAssignment.EndTimeForTreatment.Value - call.OpeningTime : null,
                Status = Tools.GetCallStatus(call),
                TotalAssignment = allAssignments.Count(a => a.IdOfRunnerCall == call.IdCall)
            };
        });

        //filter by:
        if (filterBy != null && filterValue != null)
        {
            callList = callList.Where(c => c.GetType().GetProperty(filterBy.ToString())?.GetValue(c)?.Equals(filterValue) == true);
        }

        //sort by:
        callList = sortBy switch
        {
            BO.CallInListFields.StartTime => callList.OrderBy(c => c.StartTime),
            BO.CallInListFields.TimeToCompleteTreatment => callList.OrderBy(c => c.TimeTocompleteTreatment ?? TimeSpan.Zero),
            BO.CallInListFields.LastUpdateBy => callList.OrderBy(c => c.LastUpdateBy ?? string.Empty),
            BO.CallInListFields.Status => callList.OrderBy(c => c.Status),
            _ => callList.OrderBy(c => c.CallId)
        };
        return callList;
    }
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
}
