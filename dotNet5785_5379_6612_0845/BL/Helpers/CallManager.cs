using BL.BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Helpers;

internal class CallManager
{
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get;
    internal static IEnumerable<BO.Call> ConvertToBOCalls(IEnumerable<DO.Call> doCalls)
    {
        return doCalls.Select(doCall => new BO.Call
        {
            IdCall = doCall.IdCall,
            CallType = (CallTypes)doCall.CallTypes, // המרת סוג קריאה
            CallDescription = doCall.CallDescription,
            AddressOfCall = doCall.CallAddress != null ? doCall.CallAddress : throw new ArgumentNullException(nameof(doCall.CallAddress)),
            CallLongitude = doCall.CallLongitude,
            CallLatitude = doCall.CallLatitude,
            OpeningTime = (DateTime)doCall.OpeningTime,
            MaxFinishTime = doCall.MaxFinishTime
            //,
            //StatusCallType = (StatusCallType)Tools.GetCallStatus(doCall.IdCall),
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
    //internal static BO.StatusCallType CalculateCallStatus(int callId)
    //{
    //    try
    //    {
    //        //// Get the call from database
    //        var call = s_dal.Call.Read(callId);
    //        if (call == null)
    //            throw new ArgumentException($"Call with ID={callId} does not exist.");

    //        // Get all assignments for this call
    //        var assignments = s_dal.Assignment.ReadAll(a => a.IdOfRunnerCall == callId);
    //        if (assignments == null)
    //            throw new ArgumentException($"Call with ID={callId} does not has assignment.");
    //        // If there are no assignments at all
    //        if (!assignments.Any())
    //        {
    //            // Check if call has expired
    //            if (ClockManager.Now > call.MaxFinishTime)
    //                return BO.StatusCallType.Expired;

    //            // Check if call is at risk (less than 30 minutes to expiration)
    //            var timeToExpiration = call.MaxFinishTime - ClockManager.Now;
    //            if (timeToExpiration?.TotalMinutes <= 30)
    //                return BO.StatusCallType.openInRisk;

    //            return BO.StatusCallType.open;
    //        }

    //        // Get the latest active assignment (no EndTime)
    //        var activeAssignment = assignments.FirstOrDefault(a => a.EndTimeForTreatment == null);
    //        // If there's no active assignment but there are completed assignments
    //        if (activeAssignment == null)
    //        {
    //            // Check if any assignment was completed successfully
    //            var successfulAssignment = assignments.Any(a => a.FinishCallType == DO.FinishCallType.TakenCareof);
    //            return successfulAssignment ? BO.StatusCallType.closed : BO.StatusCallType.open;
    //        }
    //        // There is an active assignment - check if it's at risk
    //        var remainingTime = call.MaxFinishTime - ClockManager.Now;
    //        if (remainingTime?.TotalMinutes <= 30)
    //            return BO.StatusCallType.HandlingInRisk;

    //        return BO.StatusCallType.inHandling;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlInvalidOperationException($"Error calculating call status: {ex.Message}", ex);
    //    }
    //}

}
