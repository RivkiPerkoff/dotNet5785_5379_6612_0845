using BL.BO;
namespace BL.BIApi;
public interface ICall
{
    int[] CallListAmount();

    //לא גמור
    CallInList RequestCallListRead(Enum toFiltering, object? filtering, Enum toSort);
    void UpdateCallDetail(Call callObject);
    void DeleteCall(int id);
    void AddCall(Call callObject);
    //לא גמור
    ClosedCallInList RequestListOfClosedCallsVolunteer(int volunteerId, CallTypes? callType, Enum a);
    //לא גמור
    ClosedCallInList RequestListOfOpenCallSelectedByvolunteer(int volunteerId, Enum? a, Enum? b);
    void UpdateEndTreatment(int id, int idAsignment);
    void UpdateCancelTreatment(int id, int idAsignment);
    void ChoosingCallForTreatment(int id, int idAsignment);

}
