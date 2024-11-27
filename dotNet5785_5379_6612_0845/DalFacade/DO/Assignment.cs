namespace DO;

public record Assignment
(
    int NextAssignmentId,
    int IdOfRunnerCall,
    int VolunteerId,
    TypeOfEndTime TypeOfEndTime,

    DateTime EntryTimeForTreatment,
    DateTime? EndTimeForTreatment = null
)
{
    public Assignment() : this(0, 0, 0, TypeOfEndTime.treated, DateTime.Now) { }
}
