using BL.BO;
using DalApi;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BL.Helpers;

internal class CallManager
{
    internal static ObserverManager Observers = new();
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get;
    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;
    private static int s_periodicCounter = 0;
    internal static void SendEmailToVolunteerOnAssignmentCancellation(int volunteerId, int callId)
    {
        DO.Volunteer volunteer;
        DO.Call call;

        lock (AdminManager.BlMutex)
        {
            volunteer = s_dal.Volunteer.Read(volunteerId)
                ?? throw new BO.BlGeneralDatabaseException("Volunteer not found.");

            call = s_dal.Call.Read(callId)
                ?? throw new BO.BlGeneralDatabaseException("Call not found.");
        }

        string subject = "Call Assignment Cancelled";
        string body = $@"
Hello {volunteer.Name},

The call you were handling has been cancelled by a manager.
Call Details:
- Call ID: {call.IdCall}
- Call Address: {call.CallAddress}
- Opening Time: {call.OpeningTime}
- Description: {call.CallDescription}

You are no longer assigned to this call.

Best regards,  
Call Management System Of TrampIst";

        Tools.SendEmail(volunteer.EmailOfVolunteer, subject, body);
    }

    internal static void SendEmailWhenCallOpened(BO.Call call)
    {
        List<DO.Volunteer> volunteers;
        lock (AdminManager.BlMutex)
            volunteers = s_dal.Volunteer.ReadAll().ToList();
        foreach (var item in volunteers)
        {
            double distance = Tools.DistanceCalculation(item.AddressVolunteer, call.AddressOfCall);

            if (item.MaximumDistanceForReceivingCall.HasValue && distance > item.MaximumDistanceForReceivingCall.Value)
                continue; // המתנדב רחוק מדי

            string subject = "Opening call";
            string body = $@"
      Hello {item.Name},

     A new call has been opened in your area.
      Call Details:
      - Call ID: {call.IdCall}
      - Call Type: {call.CallType}
      - Call Address: {call.AddressOfCall}
      - Opening Time: {call.OpeningTime}
      - Description: {call.CallDescription}
      - Entry Time for Treatment: {call.MaxFinishTime}
      - Call Status: {call.StatusCallType}

      If you wish to handle this call, please log into the system.

      Best regards,  
     Call Management System Of TrampIst";

            Tools.SendEmail(item.EmailOfVolunteer, subject, body);
        }
    }

    internal static IEnumerable<BO.Call> ConvertToBOCalls(IEnumerable<DO.Call> doCalls)
    {
        return doCalls.Select(doCall => new BO.Call
        {
            IdCall = doCall.IdCall,
            CallType = ToBOCallType(doCall.CallTypes),
            CallDescription = doCall.CallDescription,
            AddressOfCall = doCall.CallAddress ?? throw new ArgumentNullException(nameof(doCall.CallAddress)),
            CallLongitude = doCall.CallLongitude,
            CallLatitude = doCall.CallLatitude,
            OpeningTime = (DateTime)doCall.OpeningTime,
            MaxFinishTime = doCall.MaxFinishTime,
            StatusCallType = (StatusCallType)Tools.GetCallStatus(doCall),
        });
    }

    internal static object? GetFieldValue(CallInList call, CallInListFields field)
    {
        return field switch
        {
            CallInListFields.Id => call.Id,
            CallInListFields.CallId => call.CallId,
            CallInListFields.CallType => call.CallType,
            CallInListFields.StartTime => call.StartTime,
            CallInListFields.TimeToEnd => call.TimeToEnd,
            CallInListFields.LastUpdateBy => call.LastUpdateBy,
            CallInListFields.TimeToCompleteTreatment => call.TimeTocompleteTreatment,
            CallInListFields.Status => call.Status,
            CallInListFields.TotalAssignment => call.TotalAssignment,
            _ => null
        };
    }

    public static IEnumerable<BO.ClosedCallInList> CreateClosedCallList(IEnumerable<DO.Call> calls, IEnumerable<DO.Assignment> assignments)
    {
        return calls.Select(call =>
        {
            var assignment = assignments
                .Where(a => a.IdOfRunnerCall == call.IdCall)
                .OrderByDescending(a => a.EntryTimeForTreatment)
                .FirstOrDefault();

            return new BO.ClosedCallInList
            {
                Id = call.IdCall,
                CallTypes = ToBOCallType(call.CallTypes),
                OpeningTime = (DateTime)call.OpeningTime,
                Address = call.CallAddress,
                EntryTimeForTreatment = assignment?.EntryTimeForTreatment ?? DateTime.MinValue,
                EndTimeForTreatment = assignment?.EndTimeForTreatment,
                FinishCallType = assignment?.FinishCallType != null
                    ? ToBOTreatmentEndType(assignment.FinishCallType.Value)
                    : null
            };
        });
    }

    public static DO.CallTypes ToDOCallType(BO.CallTypes type) => type switch
    {
        BO.CallTypes.ManDriver => DO.CallTypes.ManDriver,
        BO.CallTypes.WomanDriver => DO.CallTypes.WomanDriver,
        _ => DO.CallTypes.None
    };

    public static BO.CallTypes ToBOCallType(DO.CallTypes type) => type switch
    {
        DO.CallTypes.ManDriver => BO.CallTypes.ManDriver,
        DO.CallTypes.WomanDriver => BO.CallTypes.WomanDriver,
        _ => BO.CallTypes.None
    };

    public static TreatmentEndType ToBOTreatmentEndType(DO.FinishCallType type) => type switch
    {
        DO.FinishCallType.TakenCareof => TreatmentEndType.TakenCareof,
        DO.FinishCallType.CanceledByVolunteer => TreatmentEndType.CanceledByVolunteer,
        DO.FinishCallType.CanceledByManager => TreatmentEndType.CanceledByManager,
        DO.FinishCallType.Expired => TreatmentEndType.Expired,
        _ => TreatmentEndType.None
    };

    public static int GetCountOfCompletedCalls(int volunteerId)
    {
        lock (AdminManager.BlMutex)
        {
            var assignments = s_dal.Assignment.ReadAll();
            return assignments.Count(a => a.VolunteerId == volunteerId && a.FinishCallType == DO.FinishCallType.TakenCareof);
        }
    }

    public static int GetCountOfSelfCancelledCalls(int volunteerId)
    {
        lock (AdminManager.BlMutex)
        {
            var assignments = s_dal.Assignment.ReadAll();
            return assignments.Count(a => a.VolunteerId == volunteerId && a.FinishCallType == DO.FinishCallType.CanceledByVolunteer);
        }
    }

    public static int GetCountOfExpiredCalls(int volunteerId)
    {
        lock (AdminManager.BlMutex)
        {
            var assignments = s_dal.Assignment.ReadAll();
            return assignments.Count(a => a.VolunteerId == volunteerId && a.FinishCallType == DO.FinishCallType.Expired);
        }
    }

    public static int? GetCallInTreatment(int volunteerId)
    {
        lock (AdminManager.BlMutex)
        {
            var assignments = s_dal.Assignment.ReadAll();
            return assignments
                .Where(a => a.VolunteerId == volunteerId && a.EndTimeForTreatment == null)
                .Select(a => (int?)a.IdOfRunnerCall)
                .FirstOrDefault();
        }
    }

    internal static void PeriodicCallUpdates(DateTime oldClock, DateTime newClock)
    {
        Thread.CurrentThread.Name = $"Periodic{++s_periodicCounter}";

        List<DO.Call> expiredCalls;
        List<int> callsToNotify = new();

        // שלב 1: קבלת קריאות שפגו תוקף - הפיכה מיידית ל־List
        lock (AdminManager.BlMutex)
        {
            expiredCalls = s_dal.Call.ReadAll(c => c.MaxFinishTime.HasValue && c.MaxFinishTime.Value <= newClock).ToList();
        }

        foreach (var call in expiredCalls)
        {
            List<DO.Assignment> assignments;
            List<DO.Assignment> assignmentsWithNull;
            int newAssignmentId;

            // שלב 2: שליפת כל ההקצאות לקריאה הנוכחית
            lock (AdminManager.BlMutex)
            {
                assignments = s_dal.Assignment.ReadAll(a => a.IdOfRunnerCall == call.IdCall).ToList();
                newAssignmentId = s_dal.Config.CreateAssignmentId();
            }

            // שלב 3: אם אין הקצאות בכלל - צור הקצאה אוטומטית
            if (!assignments.Any())
            {
                lock (AdminManager.BlMutex)
                {
                    s_dal.Assignment.Create(new DO.Assignment(
                        NextAssignmentId: newAssignmentId,
                        IdOfRunnerCall: call.IdCall,
                        VolunteerId: 0,
                        EntryTimeForTreatment: AdminManager.Now,
                        EndTimeForTreatment: AdminManager.Now,
                        FinishCallType: DO.FinishCallType.Expired
                    ));
                }

                callsToNotify.Add(call.IdCall); // notification אחרי ה־lock
            }

            // שלב 4: קבלת הקצאות שלא הסתיימו (FinishCallType == null)
            lock (AdminManager.BlMutex)
            {
                assignmentsWithNull = s_dal.Assignment
                    .ReadAll(a => a.IdOfRunnerCall == call.IdCall && a.FinishCallType == null)
                    .ToList();
            }

            // שלב 5: אם יש כאלה – עדכן אותם כ־Expired
            if (assignmentsWithNull.Any())
            {
                lock (AdminManager.BlMutex)
                {
                    foreach (var assignment in assignmentsWithNull)
                    {
                        s_dal.Assignment.Update(assignment with
                        {
                            EndTimeForTreatment = AdminManager.Now,
                            FinishCallType = DO.FinishCallType.Expired
                        });
                    }
                }

                callsToNotify.Add(call.IdCall); // notification אחרי ה־lock
            }
        }

        // שלב 6: שליחת התראות למשקיפים (רק מחוץ ל־lock)
        foreach (var id in callsToNotify.Distinct())
            Observers.NotifyItemUpdated(id);
    }

    internal static void SimulateCallAssignmentAndTreatment()
    {
        Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";

        LinkedList<int> callsToNotify = new();

        List<DO.Call> openCalls;
        List<DO.Volunteer> availableVolunteers;
        List<DO.Assignment> ongoingAssignments;

        // קריאה ל-DAL בתוך lock והמרה מיידית ל-List
        lock (AdminManager.BlMutex)
        {
            openCalls = s_dal.Call.ReadAll(c =>
                Tools.GetCallStatus(c) == BO.StatusCallType.open ||
                Tools.GetCallStatus(c) == BO.StatusCallType.openInRisk).ToList();

            availableVolunteers = s_dal.Volunteer.ReadAll(v => v.IsAvailable).ToList();
            ongoingAssignments = s_dal.Assignment.ReadAll(a => a.EndTimeForTreatment == null).ToList();
        }

        // שיוך מתנדבים לקריאות פתוחות
        foreach (var call in openCalls)
        {
            if (!availableVolunteers.Any()) break;

            var volunteer = availableVolunteers[s_rand.Next(availableVolunteers.Count)];

            lock (AdminManager.BlMutex)
            {
                var newAssignmentId = s_dal.Config.CreateAssignmentId();

                s_dal.Assignment.Create(new DO.Assignment(
                    NextAssignmentId: newAssignmentId,
                    IdOfRunnerCall: call.IdCall,
                    VolunteerId: volunteer.VolunteerId,
                    FinishCallType: DO.FinishCallType.None,
                    EntryTimeForTreatment: AdminManager.Now,
                    EndTimeForTreatment: null
                ));

                s_dal.Volunteer.Update(volunteer with { IsAvailable = false });
            }

            callsToNotify.AddLast(call.IdCall);
            availableVolunteers.Remove(volunteer);
        }

        // סימולציית סיום קריאות
        foreach (var assignment in ongoingAssignments)
        {
            if (s_rand.NextDouble() < 0.3)
            {
                bool updated = false;

                lock (AdminManager.BlMutex)
                {
                    s_dal.Assignment.Update(assignment with
                    {
                        EndTimeForTreatment = AdminManager.Now,
                        FinishCallType = DO.FinishCallType.TakenCareof
                    });

                    var volunteer = s_dal.Volunteer.Read(assignment.VolunteerId);
                    if (volunteer != null)
                    {
                        s_dal.Volunteer.Update(volunteer with { IsAvailable = true });
                    }

                    updated = true;
                }

                if (updated)
                    callsToNotify.AddLast(assignment.IdOfRunnerCall);
            }
        }

        // שליחת התראות למשקיפים מחוץ ל-lock
        foreach (int id in callsToNotify.Distinct())
            Observers.NotifyItemUpdated(id);
    }

}
