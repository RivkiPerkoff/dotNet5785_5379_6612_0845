using BL.BIApi;
using BL.BO;

namespace BL.BlImplementation;

internal class CallImplementation : BIApi.ICall
{
    public void AddCall(Call callObject)
    {
        throw new NotImplementedException();
    }

    public void AssignVolunteerToCall(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public void CancelCallTreatment(int requesterId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void CompleteCallTreatment(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void DeleteCall(int callId)
    {
        throw new NotImplementedException();
    }

    public int[] GetCallAmountsByStatus()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ClosedCallInList> GetClosedCallsForVolunteer(int volunteerId, CallTypes? callType, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ClosedCallInList> GetClosedCallsForVolunteer(int volunteerId, StatusCallType? callType, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CallInList> GetFilteredAndSortedCallList(Enum? filterField, object? filterValue, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenCallInList> GetOpenCallsForVolunteerSelection(int volunteerId, Enum? filterField, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public void UpdateCallDetails(Call callObject)
    {
        throw new NotImplementedException();
    }

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

}
