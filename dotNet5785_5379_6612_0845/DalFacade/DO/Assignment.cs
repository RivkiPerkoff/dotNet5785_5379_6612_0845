namespace DO;
/// <summary>
/// Represents an assignment record, which includes information about the assignment's unique ID,
/// associated call, volunteer, treatment times, and type of end time.
/// </summary>
/// <param name="NextAssignmentId">The unique ID for the next assignment to be created.</param>
/// <param name="IdOfRunnerCall">The ID of the runner's call associated with this assignment.</param>
/// <param name="VolunteerId">The ID of the volunteer assigned to this task.</param>
/// <param name="FinishCallType">The type of end time, indicating whether the task was TakenCareof or ended in a different way.</param>
/// <param name="EntryTimeForTreatment">The entry time when the treatment process starts.</param>
/// <param name="EndTimeForTreatment">The end time when the treatment process finishes (optional, defaults to null).</param>
public record Assignment
(
    int NextAssignmentId,
    int IdOfRunnerCall,
    int VolunteerId,
    FinishCallType FinishCallType,
    DateTime EntryTimeForTreatment,
    DateTime? EndTimeForTreatment = null
)
{
    public Assignment() : this(0, 0, 0, FinishCallType.TakenCareof, DateTime.Now) { }

}
