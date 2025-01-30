
namespace BL.BO;
public class CallInProgress
{
    public int Id { get; init; }
    public int CallId { get; init; }
    public CallTypes CallTypes { get; set; }
    public string? Description { get; set; }
    public string CallingAddress { get; set; }
    public DateTime OpeningTime { get; set; }
    public DateTime? MaxFinishTime { get; set; }
    public DateTime EntryTimeForTreatment { get; set; }
    public double CallingDistanceFromVolunteer { get; set; }
    public RiskRangeStatus Status { get; set; }
    public override string ToString()
    {
        return $"Id: {Id}, CallId: {CallId}, CallTypes: {CallTypes}, Description: {Description}, " +
               $"CallingAddress: {CallingAddress}, OpeningTime: {OpeningTime}, MaxFinishTime: {MaxFinishTime}, " +
               $"EntryTimeForTreatment: {EntryTimeForTreatment}, CallingDistanceFromVolunteer: {CallingDistanceFromVolunteer}, " +
               $"Status: {Status}";
    }
}
