
namespace BL.BO;

public class CallInProgress
{
    public int AllocationEntityId { get; init; }
    public int CallId { get; init; }
    CallTypes callTypes { get; set; }
    public string? Description { get; set; }
    public string CallingAddress { get; set; }
    DateTime OpeningTime { get; set; }
    DateTime? MaxFinishTime { get; set; }
    DateTime EntryTimeForTreatment { get; set; }
    double CallingDistanceFromVolunteer { get; set; }
    RiskRangeStatus Status { get; set; }
    public override string ToString() => this.ToStringProperty();
}
