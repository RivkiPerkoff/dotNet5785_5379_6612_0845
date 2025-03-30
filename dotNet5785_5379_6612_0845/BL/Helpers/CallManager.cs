﻿using BL.BO;
using DalApi;
using Microsoft.VisualBasic;
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
            MaxFinishTime = doCall.MaxFinishTime,
            //,
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
            var assignment = assignments.First(a => a.IdOfRunnerCall == call.IdCall);
            return new BO.ClosedCallInList
            {
                Id = call.IdCall,
                CallTypes = (DO.CallTypes)call.CallTypes,
                OpeningTime = (DateTime)call.OpeningTime,
                Address = call.CallAddress,
                EntryTimeForTreatment = (DateTime)assignment.EntryTimeForTreatment,
                EndTimeForTreatment = assignment.EntryTimeForTreatment,
                FinishCallType = (DO.FinishCallType)assignment.FinishCallType
            };
        });
    }


    

}
