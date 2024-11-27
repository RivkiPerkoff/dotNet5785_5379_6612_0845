namespace DO;

/// <summary>
/// Represents an assignment record.
/// </summary>
public record Assignment
{
    public int NextAssignmentId { get; init; }
    public int IdOfRunnerCall { get; init; }
    public int VolunteerId { get; init; }
    public DateTime EntryTimeForTreatment { get; init; }
    public DateTime? EndTimeForTreatment { get; init; }
    public TypeOfEndTime TypeOfEndTime { get; init; }

    // קונסטרקטור ברירת מחדל
    public Assignment()
        : this(0, 0, 0, DateTime.Now, null, TypeOfEndTime.treated) { }

    // קונסטרקטור ראשי
    public Assignment(int nextAssignmentId, int idOfRunnerCall, int volunteerId, DateTime entryTimeForTreatment, DateTime? endTimeForTreatment, TypeOfEndTime typeOfEndTime)
    {
        NextAssignmentId = nextAssignmentId;
        IdOfRunnerCall = idOfRunnerCall;
        VolunteerId = volunteerId;
        EntryTimeForTreatment = entryTimeForTreatment;
        EndTimeForTreatment = endTimeForTreatment;
        TypeOfEndTime = typeOfEndTime;
    }

    // קונסטרקטור מותאם אישית
    public Assignment(DateTime randomTime, DateTime? endTimeForTreatment, TypeOfEndTime typeOfEndTime)
        : this(0, 0, 0, randomTime, endTimeForTreatment, typeOfEndTime) { }
}