using BL.BIApi;
using BL.BO;
using BL.Helpers;
using DO;
using NSubstitute.Core;

namespace BL.BlImplementation;

internal class CallImplementation : BIApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public int[] GetCallAmounts()
    {
        IEnumerable<DO.Call> doCalls = _dal.Call.ReadAll();
        IEnumerable<BO.Call> boCalls = CallManager.ConvertToBOCalls(doCalls);

        int enumSize = Enum.GetValues(typeof(StatusCallType)).Length; // Get the number of enum values
        int[] statusCounts = new int[enumSize]; // Create an array matching the enum size

        foreach (var group in boCalls.GroupBy(call => (int)call.StatusCallType))
        {
            statusCounts[group.Key] = group.Count(); // Using GroupBy.
        }

        return statusCounts;
    }
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
            CallAssignInLists = null 
        };
    }
    public IEnumerable<CallInList> GetFilteredAndSortedCallList(CallInListFields? filterField, object? filterValue, CallInListFields? sortField)
    {
        var calls = _dal.Call.ReadAll();
        var assignments = _dal.Assignment.ReadAll();
        var callList = calls.Select(call =>
        {
            var lastAssignment = assignments
                .Where(a => a.IdOfRunnerCall == call.IdCall)
                .OrderByDescending(a => a.EntryTimeForTreatment)
                .FirstOrDefault();
            return new CallInList
            {
                Id = call.IdCall,
                CallId = call.IdCall,
                CallType = (BO.CallTypes)call.CallTypes,
                StartTime = (DateTime)call.OpeningTime,
                TimeToEnd = call.MaxFinishTime.HasValue ? call.MaxFinishTime - DateTime.Now : null,
                LastUpdateBy = lastAssignment != null ? $"Volunteer {lastAssignment.VolunteerId}" : null,
                TimeTocompleteTreatment = lastAssignment?.EndTimeForTreatment.HasValue == true
                    ? lastAssignment.EndTimeForTreatment - lastAssignment.EntryTimeForTreatment
                    : null,
                Status = Tools.GetCallStatus(call.IdCall),
                TotalAssignment = assignments.Count(a => a.IdOfRunnerCall == call.IdCall)
            };
        }).ToList();

        if (filterField != null && filterValue != null)
        {
            callList = callList.Where(call =>
                CallManager.GetFieldValue(call, filterField.Value)?.Equals(filterValue) == true
            ).ToList();
        }
        callList = sortField switch
        {
            null => callList.OrderBy(c => c.CallId).ToList(),
            _ => callList.OrderBy(c => CallManager.GetFieldValue(c, sortField.Value)).ToList()
        };
        return callList;
    }
    public void DeleteCall(int callId)
    {
        try
        {
            var call = _dal.Call.Read(callId) ?? throw new DO.DalDoesNotExistException($"Call with ID {callId} not found.");
            var callStatus = Tools.GetCallStatus(call.IdCall);
            var assignments = _dal.Assignment.ReadAll()
            .Any(a => a.IdOfRunnerCall == callId);
            if (callStatus != StatusCallType.open || assignments)
                throw new BlGeneralDatabaseException("Cannot delete this call. Only open calls that have never been assigned can be deleted.");
            _dal.Call.Delete(callId);
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

    public IEnumerable<ClosedCallInList> GetClosedCallsForVolunteer(int volunteerId, BO.CallTypes? filterType, ClosedCallInListFields? sortField)
    {
        var volunteerAssignments = _dal.Assignment.ReadAll()
            .Where(a => a.VolunteerId == volunteerId)
            .Select(a => a.IdOfRunnerCall);

        var closedCalls = _dal.Call.ReadAll()
            .Where(c => volunteerAssignments.Contains(c.IdCall))
            .Join(_dal.Assignment.ReadAll(),
                  call => call.IdCall,
                  assignment => assignment.IdOfRunnerCall,
                  (call, assignment) => new ClosedCallInList
                  {
                      Id = call.IdCall,
                      CallTypes = call.CallTypes,
                      Address = call.CallAddress ?? string.Empty,
                      OpeningTime = (DateTime)call.OpeningTime,
                      EntryTimeForTreatment = assignment.EntryTimeForTreatment ?? default,
                      EndTimeForTreatment = assignment.EndTimeForTreatment,
                      FinishCallType = assignment.FinishCallType
                  })
            .Where(c => c.FinishCallType != null);

        if (filterType.HasValue)
        {
            closedCalls = closedCalls.Where(c => (BO.CallTypes)c.CallTypes == filterType.Value);
        }

        if (sortField.HasValue)
        {
            closedCalls = closedCalls.OrderBy(c => c.GetType()
                                                    .GetProperty(sortField.ToString())?
                                                    .GetValue(c))
                                     .ThenBy(c => c.Id);
        }
        else
        {
            closedCalls = closedCalls.OrderBy(c => c.Id);
        }

        return closedCalls.ToList(); // הפיכת IEnumerable ל- List
    }
    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteerSelection(int volunteerId, BO.CallTypes? filterField, OpenCallInListFields? sortField)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");

            var volunteerAssignments = _dal.Assignment.ReadAll()
                .Where(a => a.VolunteerId == volunteerId) // סינון על פי ת.ז של המתנדב
                .Select(a => a.IdOfRunnerCall); // קבלת IdOfRunnerCall מתוך ההקצאות

            // שלב 2: קריאה ל- DAL על מנת להחזיר את הקריאות הפתוחות עם סטטוס מתאים
            var openCalls = _dal.Call.ReadAll()
                .Where(c => volunteerAssignments.Contains(c.IdCall) &&
                            (Tools.GetCallStatus(c.IdCall) == StatusCallType.open || Tools.GetCallStatus(c.IdCall) == StatusCallType.openInRisk)) // סינון לפי סטטוס "פתוחה" או "פתוחה בסיכון"
                .Join(_dal.Assignment.ReadAll(), // ביצוע Join בין קריאות להקצאות
                      call => call.IdCall, // חיבור לפי IdCall
                      assignment => assignment.IdOfRunnerCall, // חיבור לפי IdOfRunnerCall בהקצאה
                      (call, assignment) => new OpenCallInList // יצירת אובייקטים מסוג OpenCallInList
                      {
                          Id = call.IdCall, // ID של הקריאה
                          CallTypes = call.CallTypes, // מיפוי CallTypes ל-BO
                          CallDescription = call.CallDescription, // תיאור הקריאה
                          Address = call.CallAddress , // כתובת הקריאה
                          OpeningTime = (DateTime)call.OpeningTime, // זמן פתיחת הקריאה
                          MaxFinishTime = call.MaxFinishTime, // זמן סיום מקסימלי לקריאה
                          CallDistance = Tools.DistanceCalculation(volunteer.AddressVolunteer, call.CallAddress) // חישוב המרחק בין המתנדב לקריאה
                      });

            // שלב 3: סינון לפי סוג הקריאה אם הועבר filterField
            if (filterField.HasValue)
                openCalls = openCalls.Where(c =>
                {
                    return (BO.CallTypes)c.CallTypes == filterField.Value;
                }); // סינון לפי סוג הקריאה

            // שלב 4: מיון הקריאות לפי שדה הסדר (sortField), ברירת מחדל לפי ID של הקריאה
            openCalls = sortField.HasValue
                ? openCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString())?.GetValue(c)) // מיון לפי השדה שנבחר
                : openCalls.OrderBy(c => c.Id); // אם לא הועבר sortField, מיון לפי ID של הקריאה

            // החזרת רשימת הקריאות הממוינת והמסוננת בצורה מוחלטת
            return openCalls.ToList(); // יש להמיר את ה- IEnumerable ל- List
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the open calls list.", ex);
        }
    }
    public void CompleteCallTreatment(int volunteerId, int assignmentId)
    {
        try
        {
            // שלב 1: קריאת ההקצאה משכבת הנתונים
            var assignment = _dal.Assignment.Read(assignmentId)
                ?? throw new DO.DalDoesNotExistException($"Assignment with ID={assignmentId} does not exist.");

            // שלב 2: בדיקת הרשאה - האם המתנדב שמבקש הוא זה שעליו רשומה ההקצאה
            if (assignment.VolunteerId != volunteerId)
                throw new BO.BlPermissionException("Unauthorized: The volunteer is not assigned to this task.");

            // שלב 3: בדיקה שההקצאה עדיין פתוחה (כלומר, לא טופלה או בוטלה)
            if (assignment.FinishCallType.HasValue)
                throw new BO.BlInvalidOperationException("Assignment has already been completed or expired or canceled.");

            // שלב 4: יצירת אובייקט מעודכן עם סטטוס "טופלה" וזמן סיום
            var updatedAssignment = assignment with
            {
                FinishCallType = FinishCallType.TakenCareof,
                EndTimeForTreatment = ClockManager.Now
            };

            // שלב 5: ביצוע עדכון לשכבת הנתונים
            _dal.Assignment.Update(updatedAssignment);
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

    public void ChoosingCallForTreatment(int volunteerId, int callId)
    {
        try
        {
            // שלב 1: שליפת נתוני המתנדב והקריאה מה-DAL
            var volunteer = _dal.Volunteer.Read(volunteerId)
                ?? throw new BO.BlGeneralDatabaseException($"Volunteer with ID={volunteerId} does not exist.");

            var call = _dal.Call.Read(callId)
                ?? throw new BO.BlGeneralDatabaseException($"Call with ID={callId} does not exist.");

            // שלב 2: בדיקת סטטוס הקריאה
            var callStatus = Tools.GetCallStatus(callId);
            if (callStatus != StatusCallType.open && callStatus != StatusCallType.openInRisk)
                throw new BO.BlInvalidOperationException("Call is already assigned or has expired.");

            // שלב 3: בדיקה שאין הקצאה פתוחה על הקריאה
            var existingAssignment = _dal.Assignment.ReadAll()
                .FirstOrDefault(a => a.IdOfRunnerCall == callId && a.EndTimeForTreatment == null);

            if (existingAssignment != null)
                throw new BO.BlInvalidOperationException("Call is already being handled by another volunteer.");

            // שלב 4: יצירת הקצאה חדשה
            var newAssignment = new DO.Assignment(
                NextAssignmentId: 0, // ישות חדשה - ה-DAL אמור לקבוע מזהה
                IdOfRunnerCall: callId,
                VolunteerId: volunteerId,
                FinishCallType: null, // לא נקבע סוג סיום בשלב זה
                EntryTimeForTreatment: ClockManager.Now,
                EndTimeForTreatment: null
            );

            _dal.Assignment.Create(newAssignment); // הוספת ההקצאה למסד הנתונים
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

}
