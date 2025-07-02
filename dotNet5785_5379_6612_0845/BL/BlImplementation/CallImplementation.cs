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
        try
        {
            IEnumerable<DO.Call> doCalls;
            lock (AdminManager.BlMutex)
                doCalls = _dal.Call.ReadAll();

            //IEnumerable<DO.Call> doCalls = _dal.Call.ReadAll();
            IEnumerable<BO.Call> boCalls = CallManager.ConvertToBOCalls(doCalls);

            int enumSize = Enum.GetValues(typeof(StatusCallType)).Length;
            int[] statusCounts = new int[enumSize];

            foreach (var group in boCalls.GroupBy(call => (int)call.StatusCallType))
            {
                statusCounts[group.Key] = group.Count();
            }
            return statusCounts;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while retrieving call amounts.", ex);
        }
    }

    /// <summary>
    /// Retrieves the detailed information of a specific call by its ID.
    /// </summary>
    /// <param name="CallId">The ID of the call to retrieve.</param>
    /// <returns>A BO.Call object containing the details of the specified call.</returns>
    /// <exception cref="Exception">Thrown if the call with the given ID is not found.</exception>
    //public BO.Call GetCallDetails(int callId)
    //{
    //    var call = _dal.Call.Read(callId);

    //    if (call == null)
    //        throw new Exception($"Call with ID {callId} not found.");

    //    var assignments = _dal.Assignment.ReadAll(a => a.IdOfRunnerCall == callId)
    //        .OrderBy(a => a.EntryTimeForTreatment)
    //        .Select(a =>
    //        {
    //            var volunteer = _dal.Volunteer.Read(a.VolunteerId);
    //            return new BO.CallAssignInList
    //            {
    //                VolunteerId = a.VolunteerId,
    //                VolunteerName = volunteer?.Name ?? "לא ידוע",
    //                EntryTimeForTreatment = a.EntryTimeForTreatment ?? DateTime.MinValue,
    //                EndTimeForTreatment = a.EndTimeForTreatment,
    //                TreatmentEndType = (BO.TreatmentEndType?)a.FinishCallType // בהנחה שיש התאמה
    //            };
    //        }).ToList();

    //    return new BO.Call
    //    {
    //        IdCall = call.IdCall,
    //        CallType = CallManager.ToBOCallType(call.CallTypes),
    //        CallDescription = call.CallDescription,
    //        AddressOfCall = call.CallAddress,
    //        CallLongitude = call.CallLongitude,
    //        CallLatitude = call.CallLatitude,
    //        OpeningTime = call.OpeningTime ?? DateTime.MinValue,
    //        MaxFinishTime = call.MaxFinishTime,
    //        CallAssignInLists = assignments,
    //        StatusCallType = Tools.GetCallStatus(call)
    //    };

    //}
    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            DO.Call call;
            lock (AdminManager.BlMutex)
                call = _dal.Call.Read(callId);

            if (call == null)
                throw new BO.BlDoesNotExistException($"Call with ID {callId} not found.");

            List<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)
                assignments = _dal.Assignment.ReadAll(a => a.IdOfRunnerCall == callId).ToList();

            List<BO.CallAssignInList> assignmentList = new();

            foreach (var a in assignments.OrderBy(a => a.EntryTimeForTreatment))
            {
                DO.Volunteer volunteer;
                lock (AdminManager.BlMutex)
                    volunteer = _dal.Volunteer.Read(a.VolunteerId);

                assignmentList.Add(new BO.CallAssignInList
                {
                    VolunteerId = a.VolunteerId,
                    VolunteerName = volunteer?.Name ?? "לא ידוע",
                    EntryTimeForTreatment = a.EntryTimeForTreatment ?? DateTime.MinValue,
                    EndTimeForTreatment = a.EndTimeForTreatment,
                    TreatmentEndType = (BO.TreatmentEndType?)a.FinishCallType
                });
            }

            return new BO.Call
            {
                IdCall = call.IdCall,
                CallType = CallManager.ToBOCallType(call.CallTypes),
                CallDescription = call.CallDescription,
                AddressOfCall = call.CallAddress,
                CallLongitude = call.CallLongitude,
                CallLatitude = call.CallLatitude,
                OpeningTime = call.OpeningTime ?? DateTime.MinValue,
                MaxFinishTime = call.MaxFinishTime,
                CallAssignInLists = assignmentList,
                StatusCallType = Tools.GetCallStatus(call)
            };
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Call not found in data layer.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while retrieving call details.", ex);
        }
    }

    /// <summary>
    /// Deletes a specific call by its ID, only if the call is open and has no assignments.
    /// </summary>
    /// <param name="callId">The ID of the call to delete.</param>
    /// <exception cref="BlGeneralDatabaseException">Thrown if the call cannot be deleted.</exception>
    public void DeleteCall(int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            //var call = _dal.Call.Read(callId) ?? throw new DO.DalDoesNotExistException($"Call with ID {callId} not found.");
            //var callStatus = Tools.GetCallStatus(call);
            //var assignments = _dal.Assignment.ReadAll(a => a.IdOfRunnerCall == callId);
            //if ((callStatus != StatusCallType.open && callStatus != StatusCallType.openInRisk) || assignments.Any())
            //    throw new BlGeneralDatabaseException("Cannot delete this call. Only open calls that have never been assigned can be deleted.");
            //_dal.Call.Delete(callId);
            //CallManager.Observers.NotifyListUpdated(); //stage 5
            lock (AdminManager.BlMutex)
            {
                var call = _dal.Call.Read(callId)
                    ?? throw new DO.DalDoesNotExistException($"Call with ID {callId} not found.");

                var assignments = _dal.Assignment.ReadAll(a => a.IdOfRunnerCall == callId);

                if ((Tools.GetCallStatus(call) != StatusCallType.open && Tools.GetCallStatus(call) != StatusCallType.openInRisk) || assignments.Any())
                    throw new BlGeneralDatabaseException("Cannot delete this call. Only open calls that have never been assigned can be deleted.");

                _dal.Call.Delete(callId);
            }
            CallManager.Observers.NotifyListUpdated();
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
        AdminManager.ThrowOnSimulatorIsRunning();
        try
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
                CallTypes: CallManager.ToDOCallType(callObject.CallType)
            );
            lock (AdminManager.BlMutex)
                _dal.Call.Create(callDO);
            CallManager.Observers.NotifyListUpdated();
            var boCall = GetCallDetails(callDO.IdCall);
            CallManager.SendEmailWhenCallOpened(boCall);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while adding the call to the database.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while adding the call.", ex);
        }
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
            //var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.EndTimeForTreatment != null);  // Only closed calls

            //var callIds = assignments.Select(a => a.IdOfRunnerCall).Distinct();
            //var calls = _dal.Call.ReadAll(c => callIds.Contains(c.IdCall));
            List<DO.Assignment> assignments;
            List<DO.Call> calls;

            lock (AdminManager.BlMutex)
            {
                assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.EndTimeForTreatment != null).ToList();
                var callIds = assignments.Select(a => a.IdOfRunnerCall).Distinct().ToList();
                calls = _dal.Call.ReadAll(c => callIds.Contains(c.IdCall)).ToList();
            }

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

    //public IEnumerable<BO.ClosedCallInList> GetClosedCallsForVolunteer(int volunteerId, BO.CallTypes? filterType = null, BO.ClosedCallInListFields? sortField = null)
    //{
    //    try
    //    {
    //        // כל ההקצאות של המתנדב
    //        var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);

    //        // מזהה הקריאות שאליהן הוא שויך
    //        var callIds = assignments.Select(a => a.IdOfRunnerCall).Distinct();

    //        // כל הקריאות האלו מסוג DO
    //        var dalCalls = _dal.Call.ReadAll(c => callIds.Contains(c.IdCall));

    //        // סינון לקריאות שהסתיימו בכל דרך שהיא
    //        var closedCalls = dalCalls.Where(c =>
    //            c.GetCallStatus() == BO.StatusCallType.closed ||
    //            c.GetCallStatus() == BO.StatusCallType.Expired);

    //        // יצירת רשימת תצוגה - תוצאה היא IEnumerable<BO.ClosedCallInList>
    //        var closedCallList = CallManager.CreateClosedCallList(closedCalls, assignments);

    //        // סינון לפי סוג קריאה אם נבחר
    //        if (filterType.HasValue)
    //        {
    //            closedCallList = closedCallList.Where(c => (BO.CallTypes)c.CallTypes == filterType.Value);
    //        }

    //        // מיון
    //        return sortField switch
    //        {
    //            BO.ClosedCallInListFields.CallTypes => closedCallList.OrderBy(c => c.CallTypes),
    //            BO.ClosedCallInListFields.Address => closedCallList.OrderBy(c => c.Address),
    //            BO.ClosedCallInListFields.OpeningTime => closedCallList.OrderBy(c => c.OpeningTime),
    //            BO.ClosedCallInListFields.EntryTimeForTreatment => closedCallList.OrderBy(c => c.EntryTimeForTreatment),
    //            BO.ClosedCallInListFields.EndTimeForTreatment => closedCallList.OrderBy(c => c.EndTimeForTreatment),
    //            BO.ClosedCallInListFields.FinishCallType => closedCallList.OrderBy(c => c.FinishCallType),
    //            _ => closedCallList.OrderBy(c => c.Id)
    //        };
    //    }
    //    catch (DO.DalDoesNotExistException ex)
    //    {
    //        throw new BO.BlDoesNotExistException($"Could not find data for volunteer {volunteerId}", ex);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlDoesNotExistException("An error occurred while retrieving closed calls", ex);
    //    }
    //}

    /// <summary>
    /// Retrieves a list of open calls assigned to a specific volunteer, optionally filtered and sorted.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="filterField">An optional filter for the type of calls to return.</param>
    /// <param name="sortField">An optional field to sort the returned calls by.</param>
    /// <returns>A list of OpenCallInList objects representing the open calls for the volunteer.</returns>
    //public IEnumerable<OpenCallInList> GetOpenCallsForVolunteerSelection(int volunteerId, BO.CallTypes? filterField, OpenCallInListFields? sortField)
    //{
    //    try
    //    {
    //        var volunteer = _dal.Volunteer.Read(volunteerId)
    //                        ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
    //        if (string.IsNullOrWhiteSpace(volunteer.AddressVolunteer))
    //            throw new BO.BlInvalidFormatException("Cannot calculate distance - volunteer address is missing or invalid.");
    //        var openCalls = _dal.Call.ReadAll()
    //     .Where(c => Tools.GetCallStatus(c) == StatusCallType.open || Tools.GetCallStatus(c) == StatusCallType.openInRisk)
    //     .Select(c =>
    //     {
    //         if (string.IsNullOrWhiteSpace(c.CallAddress))
    //             throw new BO.BlInvalidFormatException($"Cannot calculate distance - call with ID {c.IdCall} has invalid address.");

    //         return new OpenCallInList
    //         {
    //             Id = c.IdCall,
    //             CallTypes = c.CallTypes,
    //             CallDescription = c.CallDescription,
    //             Address = c.CallAddress,
    //             OpeningTime = (DateTime)c.OpeningTime,
    //             MaxFinishTime = c.MaxFinishTime,
    //             CallDistance = Tools.DistanceCalculation(volunteer.AddressVolunteer, c.CallAddress)
    //         };
    //     });
    //        if (filterField.HasValue)
    //            openCalls = openCalls.Where(c => (BO.CallTypes)c.CallTypes == filterField.Value);

    //        openCalls = sortField switch
    //        {
    //            BO.OpenCallInListFields.CallTypes => openCalls.OrderBy(c => c.CallTypes),
    //            BO.OpenCallInListFields.CallDescription => openCalls.OrderBy(c => c.CallDescription),
    //            BO.OpenCallInListFields.Address => openCalls.OrderBy(c => c.Address),
    //            BO.OpenCallInListFields.OpeningTime => openCalls.OrderBy(c => c.OpeningTime),
    //            BO.OpenCallInListFields.MaxFinishTime => openCalls.OrderBy(c => c.MaxFinishTime),
    //            BO.OpenCallInListFields.Calldistance => openCalls.OrderBy(c => c.CallDistance),
    //            _ => openCalls.OrderBy(c => c.Id)
    //        };

    //        return openCalls.ToList();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the open calls list.", ex);
    //    }
    //}
    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteerSelection(int volunteerId, BO.CallTypes? filterField, OpenCallInListFields? sortField)
    {
        try
        {
            DO.Volunteer volunteer;
            List<DO.Call> calls;

            lock (AdminManager.BlMutex)
            {
                volunteer = _dal.Volunteer.Read(volunteerId)
                    ?? throw new BO.BlGeneralDatabaseException($"Volunteer with ID={volunteerId} does not exist.");
            }
            //lock (AdminManager.BlMutex)
                calls = _dal.Call.ReadAll().ToList();
            

            if (string.IsNullOrWhiteSpace(volunteer.AddressVolunteer))
                throw new BO.BlInvalidFormatException("Cannot calculate distance - volunteer address is missing or invalid.");

            var openCalls = calls
                .Where(c => Tools.GetCallStatus(c) == StatusCallType.open || Tools.GetCallStatus(c) == StatusCallType.openInRisk)
                .Select(c =>
                {
                    if (string.IsNullOrWhiteSpace(c.CallAddress))
                        throw new BO.BlInvalidFormatException($"Cannot calculate distance - call with ID {c.IdCall} has invalid address.");

                    return new OpenCallInList
                    {
                        Id = c.IdCall,
                        CallTypes = c.CallTypes,
                        CallDescription = c.CallDescription,
                        Address = c.CallAddress,
                        OpeningTime = (DateTime)c.OpeningTime,
                        MaxFinishTime = c.MaxFinishTime,
                        CallDistance = Tools.DistanceCalculation(volunteer.AddressVolunteer, c.CallAddress)
                    };
                });

            if (filterField.HasValue)
                openCalls = openCalls.Where(c => (BO.CallTypes)c.CallTypes == filterField.Value);

            openCalls = sortField switch
            {
                BO.OpenCallInListFields.CallTypes => openCalls.OrderBy(c => c.CallTypes),
                BO.OpenCallInListFields.CallDescription => openCalls.OrderBy(c => c.CallDescription),
                BO.OpenCallInListFields.Address => openCalls.OrderBy(c => c.Address),
                BO.OpenCallInListFields.OpeningTime => openCalls.OrderBy(c => c.OpeningTime),
                BO.OpenCallInListFields.MaxFinishTime => openCalls.OrderBy(c => c.MaxFinishTime),
                BO.OpenCallInListFields.Calldistance => openCalls.OrderBy(c => c.CallDistance),
                _ => openCalls.OrderBy(c => c.Id)
            };

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
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            //var assignment = _dal.Assignment.Read(assignmentId)
            //    ?? throw new DO.DalDoesNotExistException($"Assignment with ID={assignmentId} does not exist.");
            DO.Assignment assignment;
            lock (AdminManager.BlMutex)
                assignment = _dal.Assignment.Read(assignmentId)
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
                //_dal.Assignment.Update(updatedAssignment);
                lock (AdminManager.BlMutex)
                    _dal.Assignment.Update(updatedAssignment);

                CallManager.Observers.NotifyItemUpdated(updatedAssignment.IdOfRunnerCall); //stage 5
                CallManager.Observers.NotifyListUpdated();
                VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
                VolunteerManager.Observers.NotifyListUpdated();
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
    //public void ChoosingCallForTreatment(int volunteerId, int callId)
    //{
    //    AdminManager.ThrowOnSimulatorIsRunning();
    //    try
    //    {
    //        var volunteer = _dal.Volunteer.Read(volunteerId)
    //            ?? throw new BO.BlGeneralDatabaseException($"Volunteer with ID={volunteerId} does not exist.");

    //        var call = _dal.Call.Read(callId)
    //            ?? throw new BO.BlGeneralDatabaseException($"Call with ID={callId} does not exist.");

    //        var callStatus = Tools.GetCallStatus(call);
    //        if (callStatus != StatusCallType.open && callStatus != StatusCallType.openInRisk)
    //            throw new BO.BlInvalidOperationException("Call is already assigned or has expired.");

    //        var existingAssignment = _dal.Assignment.ReadAll()
    //            .FirstOrDefault(a => a.IdOfRunnerCall == callId && a.EndTimeForTreatment == null);
    //        var newAssignmentId = _dal.Config.CreateAssignmentId();
    //        if (existingAssignment != null)
    //            throw new BO.BlInvalidOperationException("Call is already being handled by another volunteer.");
    //        var newAssignment = new DO.Assignment(newAssignmentId, callId, volunteerId, DO.FinishCallType.None, AdminManager.Now, null);

    //        _dal.Assignment.Create(newAssignment);
    //    }
    //    catch (DO.DalDoesNotExistException ex)
    //    {
    //        throw new BO.BlGeneralDatabaseException("Database error while accessing call or volunteer data.", ex);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlGeneralDatabaseException("An error occurred while assigning the call.", ex);
    //    }
    //}
    public void ChoosingCallForTreatment(int volunteerId, int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        try
        {
            DO.Volunteer volunteer;
            DO.Call call;
            StatusCallType callStatus;
            DO.Assignment? existingAssignment;
            int newAssignmentId;

            lock (AdminManager.BlMutex)
            {
                volunteer = _dal.Volunteer.Read(volunteerId)
                    ?? throw new BO.BlGeneralDatabaseException($"Volunteer with ID={volunteerId} does not exist.");

                call = _dal.Call.Read(callId)
                    ?? throw new BO.BlGeneralDatabaseException($"Call with ID={callId} does not exist.");

                callStatus = Tools.GetCallStatus(call);

                existingAssignment = _dal.Assignment.ReadAll()
                    .FirstOrDefault(a => a.IdOfRunnerCall == callId && a.EndTimeForTreatment == null);

                newAssignmentId = _dal.Config.CreateAssignmentId();
            }

            if (callStatus != StatusCallType.open && callStatus != StatusCallType.openInRisk)
                throw new BO.BlInvalidOperationException("Call is already assigned or has expired.");

            if (existingAssignment != null)
                throw new BO.BlInvalidOperationException("Call is already being handled by another volunteer.");

            var newAssignment = new DO.Assignment(newAssignmentId, callId, volunteerId, DO.FinishCallType.None, AdminManager.Now, null);

            lock (AdminManager.BlMutex)
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
    //public void CancelCallTreatment(int requesterId, int assignmentId)
    //{
    //    var assignment = _dal.Assignment.Read(assignmentId)
    //                     ?? throw new InvalidOperationException("Assignment not found.");

    //    if (assignment.EndTimeForTreatment != null)
    //        throw new InvalidOperationException("Cannot cancel a completed treatment.");

    //    var requester = _dal.Volunteer.Read(requesterId)
    //                  ?? throw new InvalidOperationException("Volunteer not found.");

    //    if ((assignment.VolunteerId != requesterId) && (requester.Role != DO.Role.Manager))
    //        throw new InvalidOperationException("Unauthorized cancellation.");

    //    assignment = assignment with
    //    {
    //        EndTimeForTreatment = AdminManager.Now,
    //        FinishCallType = (assignment.VolunteerId == requesterId)
    //            ? DO.FinishCallType.CanceledByVolunteer
    //            : DO.FinishCallType.CanceledByManager
    //    };

    //    _dal.Assignment.Update(assignment);
    //    CallManager.Observers.NotifyItemUpdated(assignment.IdOfRunnerCall); //stage 5
    //    CallManager.Observers.NotifyListUpdated();
    //}
    //public void CancelCallTreatment(int requesterId, int assignmentId)
    //{
    //    AdminManager.ThrowOnSimulatorIsRunning();
    //    try
    //    {
    //        var assignment = _dal.Assignment.Read(assignmentId)
    //                         ?? throw new InvalidOperationException("Assignment not found.");

    //        if (assignment.EndTimeForTreatment != null)
    //            throw new InvalidOperationException("Cannot cancel a completed treatment.");

    //        var requester = _dal.Volunteer.Read(requesterId)
    //                      ?? throw new InvalidOperationException("Volunteer not found.");

    //        if ((assignment.VolunteerId != requesterId) && (requester.Role != DO.Role.Manager))
    //            throw new InvalidOperationException("Unauthorized cancellation.");

    //        assignment = assignment with
    //        {
    //            EndTimeForTreatment = AdminManager.Now,
    //            FinishCallType = (assignment.VolunteerId == requesterId)
    //                ? DO.FinishCallType.CanceledByVolunteer
    //                : DO.FinishCallType.CanceledByManager
    //        };

    //        _dal.Assignment.Update(assignment);
    //        CallManager.Observers.NotifyItemUpdated(assignment.IdOfRunnerCall);
    //        CallManager.Observers.NotifyListUpdated();
    //    }
    //    catch (DO.DalDoesNotExistException ex)
    //    {
    //        throw new BO.BlGeneralDatabaseException("Database error while accessing assignment or volunteer data.", ex);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlGeneralDatabaseException("An error occurred while cancelling the call treatment.", ex);
    //    }
    //}
    public void CancelCallTreatment(int requesterId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        try
        {
            DO.Assignment assignment;
            DO.Volunteer requester;

            lock (AdminManager.BlMutex)
            {
                assignment = _dal.Assignment.Read(assignmentId)
                    ?? throw new InvalidOperationException("Assignment not found.");

                if (assignment.EndTimeForTreatment != null)
                    throw new InvalidOperationException("Cannot cancel a completed treatment.");

                requester = _dal.Volunteer.Read(requesterId)
                    ?? throw new InvalidOperationException("Volunteer not found.");
            }

            if ((assignment.VolunteerId != requesterId) && (requester.Role != DO.Role.Manager))
                throw new InvalidOperationException("Unauthorized cancellation.");

            var updatedAssignment = assignment with
            {
                EndTimeForTreatment = AdminManager.Now,
                FinishCallType = (assignment.VolunteerId == requesterId)
                    ? DO.FinishCallType.CanceledByVolunteer
                    : DO.FinishCallType.CanceledByManager
            };

            lock (AdminManager.BlMutex)
                _dal.Assignment.Update(updatedAssignment);

            CallManager.Observers.NotifyItemUpdated(assignment.IdOfRunnerCall);
            CallManager.Observers.NotifyListUpdated();
            //VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
            VolunteerManager.Observers.NotifyListUpdated();
            if (requester.Role == DO.Role.Manager && assignment.VolunteerId != requesterId)
            {
                CallManager.SendEmailToVolunteerOnAssignmentCancellation(assignment.VolunteerId, assignment.IdOfRunnerCall);
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlGeneralDatabaseException("Database error while accessing assignment or volunteer data.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while cancelling the call treatment.", ex);
        }
    }

    /// <summary>
    /// Updates the details of an existing call.
    /// </summary>
    /// <param name="callObject">A BO.Call object containing the updated call information.</param>
    /// <exception cref="ArgumentNullException">Thrown if the callObject is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the call is not found or the update is invalid.</exception>
    //public void UpdateCallDetails(BL.BO.Call callObject)
    //{
    //    AdminManager.ThrowOnSimulatorIsRunning();
    //    if (callObject == null)
    //        throw new ArgumentNullException(nameof(callObject));

    //    try
    //    {

    //        var (latitude, longitude) = Helpers.Tools.GetCoordinatesFromAddress(callObject.AddressOfCall);
    //        callObject.CallLatitude = latitude;
    //        callObject.CallLongitude = longitude;
    //    }
    //    catch (Exception)
    //    {
    //        throw new InvalidOperationException("Invalid address: Unable to retrieve coordinates.");
    //    }

    //    if (!callObject.MaxFinishTime.HasValue || callObject.MaxFinishTime.Value <= callObject.OpeningTime)
    //        throw new InvalidOperationException("Invalid time range: Max finish time must be after opening time.");

    //    var existingCall = _dal.Call.Read(callObject.IdCall)
    //                        ?? throw new InvalidOperationException("Call not found.");

    //    var updatedCall = existingCall with
    //    {
    //        CallAddress = callObject.AddressOfCall,
    //        CallLatitude = callObject.CallLatitude,
    //        CallLongitude = callObject.CallLongitude,
    //        MaxFinishTime = callObject.MaxFinishTime,
    //        CallTypes = CallManager.ToDOCallType(callObject.CallType)
    //    };

    //    _dal.Call.Update(updatedCall);
    //    CallManager.Observers.NotifyItemUpdated(updatedCall.IdCall); //stage 5
    //    CallManager.Observers.NotifyListUpdated(); //stage 5


    //}
    public void UpdateCallDetails(BL.BO.Call callObject)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        if (callObject == null)
            throw new ArgumentNullException(nameof(callObject));

        try
        {
            var (latitude, longitude) = Helpers.Tools.GetCoordinatesFromAddress(callObject.AddressOfCall);
            callObject.CallLatitude = latitude;
            callObject.CallLongitude = longitude;

            if (!callObject.MaxFinishTime.HasValue || callObject.MaxFinishTime.Value <= callObject.OpeningTime)
                throw new InvalidOperationException("Invalid time range: Max finish time must be after opening time.");

            DO.Call existingCall;
            lock (AdminManager.BlMutex)
                existingCall = _dal.Call.Read(callObject.IdCall)
                    ?? throw new InvalidOperationException("Call not found.");

            var updatedCall = existingCall with
            {
                CallAddress = callObject.AddressOfCall,
                CallLatitude = callObject.CallLatitude,
                CallLongitude = callObject.CallLongitude,
                MaxFinishTime = callObject.MaxFinishTime,
                CallTypes = CallManager.ToDOCallType(callObject.CallType)
            };

            lock (AdminManager.BlMutex)
                _dal.Call.Update(updatedCall);

            CallManager.Observers.NotifyItemUpdated(updatedCall.IdCall);
            CallManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlGeneralDatabaseException("Database error while accessing the call.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while updating the call.", ex);
        }
    }

    /// <summary>
    /// Retrieves a filtered and sorted list of calls based on provided parameters.
    /// </summary>
    /// <param name="filterBy">An optional field to filter the calls by.</param>
    /// <param name="filterValue">The value to filter by, corresponding to the filterBy parameter.</param>
    /// <param name="sortBy">An optional field to sort the calls by.</param>
    /// <returns>A list of BO.CallInList objects representing the filtered and sorted calls.</returns>

    //public IEnumerable<CallInList> GetFilteredAndSortedCallList(CallInListFields? filterBy = null, object? filterValue = null, CallInListFields? sortBy = null)
    //{
    //    IEnumerable<DO.Call> allCalls = _dal.Call.ReadAll().ToList();
    //    IEnumerable<DO.Assignment> allAssignments = _dal.Assignment.ReadAll().ToList();

    //    IEnumerable<CallInList> callList = allCalls.Select(call =>
    //    {
    //        var assignmentsForCall = allAssignments
    //            .Where(a => a.IdOfRunnerCall == call.IdCall)
    //            .OrderByDescending(a => a.EntryTimeForTreatment)
    //            .ToList();

    //        var latestAssignment = assignmentsForCall.FirstOrDefault();

    //        var status = Tools.GetCallStatus(call);

    //        // זמן שנותר רק אם הקריאה פתוחה
    //        TimeSpan? timeLeft = null;
    //        if (status == StatusCallType.open || status == StatusCallType.openInRisk)
    //        {
    //            timeLeft = call.MaxFinishTime.HasValue
    //                ? (call.MaxFinishTime.Value > AdminManager.Now
    //                    ? call.MaxFinishTime.Value - AdminManager.Now
    //                    : TimeSpan.Zero)
    //                : null;
    //        }

    //        // משך טיפול רק אם הקריאה הסתיימה
    //        TimeSpan? treatmentDuration = null;
    //        if (status == StatusCallType.closed)
    //        {
    //            var closedAssignment = assignmentsForCall
    //                .FirstOrDefault(a => a.EndTimeForTreatment.HasValue);

    //            if (closedAssignment != null)
    //            {
    //                treatmentDuration = closedAssignment.EndTimeForTreatment.Value - closedAssignment.EntryTimeForTreatment;
    //            }
    //        }

    //        return new CallInList
    //        {
    //            Id = latestAssignment?.NextAssignmentId,
    //            CallId = call.IdCall,
    //            CallType = (BL.BO.CallTypes)call.CallTypes,
    //            StartTime = call.OpeningTime ?? DateTime.MinValue,
    //            TimeToEnd = timeLeft,
    //            TimeTocompleteTreatment = treatmentDuration,
    //            LastUpdateBy = latestAssignment != null
    //                ? _dal.Volunteer.Read(latestAssignment.VolunteerId)?.Name
    //                : null,
    //            Status = status,
    //            TotalAssignment = assignmentsForCall.Count
    //        };
    //    });

    //    // סינון
    //    if (filterBy != null && filterValue != null)
    //    {
    //        callList = callList.Where(c =>
    //            c.GetType().GetProperty(filterBy.ToString())?.GetValue(c)?.Equals(filterValue) == true);
    //    }

    //    // מיון
    //    callList = sortBy switch
    //    {
    //        CallInListFields.StartTime => callList.OrderBy(c => c.StartTime),
    //        CallInListFields.TimeToCompleteTreatment => callList.OrderBy(c => c.TimeTocompleteTreatment ?? TimeSpan.Zero),
    //        CallInListFields.LastUpdateBy => callList.OrderBy(c => c.LastUpdateBy ?? string.Empty),
    //        CallInListFields.Status => callList.OrderBy(c => c.Status),
    //        _ => callList.OrderBy(c => c.CallId)
    //    };

    //    return callList;
    //}
    public IEnumerable<CallInList> GetFilteredAndSortedCallList(CallInListFields? filterBy = null, object? filterValue = null, CallInListFields? sortBy = null)
    {
        try
        {
            IEnumerable<DO.Call> allCalls;
            IEnumerable<DO.Assignment> allAssignments;

            lock (AdminManager.BlMutex)
            {
                allCalls = _dal.Call.ReadAll().ToList();
                allAssignments = _dal.Assignment.ReadAll().ToList();
            }

            IEnumerable<CallInList> callList = allCalls.Select(call =>
            {
                var assignmentsForCall = allAssignments
                    .Where(a => a.IdOfRunnerCall == call.IdCall)
                    .OrderByDescending(a => a.EntryTimeForTreatment)
                    .ToList();

                var latestAssignment = assignmentsForCall.FirstOrDefault();

                var status = Tools.GetCallStatus(call);

                TimeSpan? timeLeft = null;
                if (status == StatusCallType.open || status == StatusCallType.openInRisk)
                {
                    timeLeft = call.MaxFinishTime.HasValue
                        ? (call.MaxFinishTime.Value > AdminManager.Now
                            ? call.MaxFinishTime.Value - AdminManager.Now
                            : TimeSpan.Zero)
                        : null;
                }

                TimeSpan? treatmentDuration = null;
                if (status == StatusCallType.closed)
                {
                    var closedAssignment = assignmentsForCall
                        .FirstOrDefault(a => a.EndTimeForTreatment.HasValue);

                    if (closedAssignment != null)
                    {
                        treatmentDuration = closedAssignment.EndTimeForTreatment.Value - closedAssignment.EntryTimeForTreatment;
                    }
                }

                string? volunteerName = null;
                if (latestAssignment != null)
                {
                    lock (AdminManager.BlMutex)
                        volunteerName = _dal.Volunteer.Read(latestAssignment.VolunteerId)?.Name;
                }

                return new CallInList
                {
                    Id = latestAssignment?.NextAssignmentId,
                    CallId = call.IdCall,
                    CallType = (BL.BO.CallTypes)call.CallTypes,
                    StartTime = call.OpeningTime ?? DateTime.MinValue,
                    TimeToEnd = timeLeft,
                    TimeTocompleteTreatment = treatmentDuration,
                    LastUpdateBy = volunteerName,
                    Status = status,
                    TotalAssignment = assignmentsForCall.Count
                };
            });

            if (filterBy != null && filterValue != null)
            {
                callList = callList.Where(c =>
                    c.GetType().GetProperty(filterBy.ToString())?.GetValue(c)?.Equals(filterValue) == true);
            }

            callList = sortBy switch
            {
                CallInListFields.StartTime => callList.OrderBy(c => c.StartTime),
                CallInListFields.TimeToCompleteTreatment => callList.OrderBy(c => c.TimeTocompleteTreatment ?? TimeSpan.Zero),
                CallInListFields.LastUpdateBy => callList.OrderBy(c => c.LastUpdateBy ?? string.Empty),
                CallInListFields.Status => callList.OrderBy(c => c.Status),
                _ => callList.OrderBy(c => c.CallId)
            };

            return callList;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the call list.", ex);
        }
    }

    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver);
    public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer);
    public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver);
    public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); 
}
