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
    TypeOfEndTime TypeOfEndTime = TypeOfEndTime.treated  // משתמשים ב-enum הקיים
)

{
    // קונסטרוקטור ברירת מחדל עם ערכים בסיסיים
    public Assignment()
        : this(0, 0, 0, DateTime.Now, null, TypeOfEndTime.treated) { }

    // קונסטרוקטור מותאם אישית נוסף
    public Assignment(DateTime randomTime, DateTime dateTime, FinishCallType finishCallType)
        : this(0, 0, 0, randomTime, null, TypeOfEndTime.treated)
    {
        RandomTime = randomTime;
        DateTime = dateTime;
        FinishCallType = finishCallType;
    }

    // תכונות נוספות שקשורות לקונסטרוקטור המותאם אישית
    public DateTime RandomTime { get; init; }
    public DateTime DateTime { get; init; }
    public FinishCallType FinishCallType { get; init; }
}