namespace DO;

/// <summary>
/// Represents an assignment record.
/// </summary>
public record Assignment
(
    int NextAssignmentId,
    int IdOfRunnerCall,
    int VolunteerId,
    DateTime EntryTimeForTreatment,
    DateTime? EndTimeForTreatment = null,
    TerminationTypeTheTreatment? TerminationTypeTheTreatment = null
)

{
    // קונסטרוקטור מותאם אישית - מאפשר יצירת אובייקט עם ערכים ברירתיים
    public Assignment()
        : this(0, 0, 0, DateTime.Now) { }

    // קונסטרוקטור מותאם אישית נוסף
    public Assignment(DateTime randomTime, DateTime dateTime, FinishCallType finishCallType)
        : this(0, 0, 0, randomTime)
    {
   
        RandomTime = randomTime;
        DateTime = dateTime;
        FinishCallType = finishCallType;
    }

    // תכונות נוספות
    public DateTime RandomTime { get; init; }
    public DateTime DateTime { get; init; }
    public FinishCallType FinishCallType { get; init; }
}
