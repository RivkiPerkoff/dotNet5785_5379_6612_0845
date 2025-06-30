using BL.BO;
using DalApi;
using Helpers;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Helpers;

internal class CallManager
{
    internal static ObserverManager Observers = new();
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get;

    /// <summary>
    /// Converts a collection of DO.Call objects to BO.Call objects.
    /// </summary>
    /// <param name="doCalls">Collection of DO.Call objects.</param>
    /// <returns>Collection of BO.Call objects.</returns>
    internal static IEnumerable<BO.Call> ConvertToBOCalls(IEnumerable<DO.Call> doCalls)
    {
        return doCalls.Select(doCall => new BO.Call
        {
            IdCall = doCall.IdCall,
            CallType = (CallTypes)doCall.CallTypes, // Convert call type
            CallDescription = doCall.CallDescription,
            AddressOfCall = doCall.CallAddress ?? throw new ArgumentNullException(nameof(doCall.CallAddress)),
            CallLongitude = doCall.CallLongitude,
            CallLatitude = doCall.CallLatitude,
            OpeningTime = (DateTime)doCall.OpeningTime,
            MaxFinishTime = doCall.MaxFinishTime,
            StatusCallType = (StatusCallType)Tools.GetCallStatus(doCall),
        });
    }

    /// <summary>
    /// Retrieves the value of a specified field from a CallInList object.
    /// </summary>
    /// <param name="call">The CallInList object.</param>
    /// <param name="field">The field to retrieve.</param>
    /// <returns>The value of the specified field, or null if not found.</returns>
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

    /// <summary>
    /// Creates a list of ClosedCallInList objects from calls and assignments.
    /// </summary>
    /// <param name="calls">Collection of DO.Call objects.</param>
    /// <param name="assignments">Collection of DO.Assignment objects.</param>
    /// <returns>Collection of BO.ClosedCallInList objects.</returns>
    //public static IEnumerable<BO.ClosedCallInList> CreateClosedCallList(IEnumerable<DO.Call> calls, IEnumerable<DO.Assignment> assignments)
    //{
    //    return calls.Select(call =>
    //    {
    //        var assignment = assignments.First(a => a.IdOfRunnerCall == call.IdCall);
    //        return new BO.ClosedCallInList
    //        {
    //            Id = call.IdCall,
    //            CallTypes = (DO.CallTypes)call.CallTypes,
    //            OpeningTime = (DateTime)call.OpeningTime,
    //            Address = call.CallAddress,
    //            EntryTimeForTreatment = (DateTime)assignment.EntryTimeForTreatment,
    //            EndTimeForTreatment = assignment.EntryTimeForTreatment,
    //            FinishCallType = (DO.FinishCallType)assignment.FinishCallType
    //        };
    //    });
    //}

    public static IEnumerable<BO.ClosedCallInList> CreateClosedCallList(IEnumerable<DO.Call> calls, IEnumerable<DO.Assignment> assignments)
    {
        return calls.Select(call =>
        {
            // לוקח את ההקצאה האחרונה של הקריאה לפי זמן כניסה
            var assignment = assignments
                .Where(a => a.IdOfRunnerCall == call.IdCall)
                .OrderByDescending(a => a.EntryTimeForTreatment)
                .FirstOrDefault();

            return new BO.ClosedCallInList
            {
                Id = call.IdCall,
                CallTypes = (DO.CallTypes)call.CallTypes,
                OpeningTime = (DateTime)call.OpeningTime,
                Address = call.CallAddress,
                EntryTimeForTreatment = assignment?.EntryTimeForTreatment ?? DateTime.MinValue,
                EndTimeForTreatment = assignment?.EndTimeForTreatment,
                FinishCallType = assignment?.FinishCallType
            };
        });
    }

    public static int GetCountOfCompletedCalls(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        if (assignments is null) return 0;
        return assignments.Count(a => a.VolunteerId == volunteerId && a.FinishCallType == DO.FinishCallType.TakenCareof);
    }
    public static int GetCountOfSelfCancelledCalls(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        if (assignments is null) return 0;
        return assignments.Count(a => a.VolunteerId == volunteerId && a.FinishCallType == DO.FinishCallType.CanceledByVolunteer);
    }
    public static int GetCountOfExpiredCalls(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        if (assignments is null) return 0;
        return assignments.Count(a => a.VolunteerId == volunteerId && a.FinishCallType == DO.FinishCallType.Expired);
    }

    public static int? GetCallInTreatment(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        if (assignments is null) return 0;
        return assignments
            .Where(a => a.VolunteerId == volunteerId && a.EndTimeForTreatment == null)
            .Select(a => (int?)a.IdOfRunnerCall)
            .FirstOrDefault();
    }
}
