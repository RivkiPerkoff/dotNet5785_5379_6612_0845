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
            CallType = doCall.CallTypes, // המרת סוג קריאה
            CallDescription = doCall.CallDescription,
            AddressOfCall = doCall.CallAddress != null ? doCall.CallAddress : throw new ArgumentNullException(nameof(doCall.CallAddress)),
            CallLongitude = doCall.CallLongitude,
            CallLatitude = doCall.CallLatitude,
            OpeningTime = doCall.OpeningTime,
            MaxFinishTime = doCall.MaxFinishTime,
            StatusCallType = BO.StatusCallType.closed,
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


}
