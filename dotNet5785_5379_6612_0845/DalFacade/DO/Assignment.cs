namespace DO;

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
    // קונסטרוקטור מותאם אישית נוסף
    public Assignment(DateTime randomTime, DateTime dateTime, FinishCallType finishCallType)
        : this(0, 0, 0, randomTime, null, TypeOfEndTime.treated) // קריאה לקונסטרוקטור הבסיסי
    {
        // כאן תוכל להוסיף לוגיקה נוספת אם צריך
        // לדוגמה, אם אתה רוצה להוסיף ערכים עבור 'FinishCallType' - תוכל להוסיף שדה או לוגיקה בהתאם.
        // אם FinishCallType הוא שדה שצריך לשייך ל-Assignment, אתה צריך להוסיף אותו כפרמטר נוסף ב-record.

        // לא ניתן לעדכן שדות ישירות בקונסטרוקטור של record, אם אתה רוצה להוסיף שדות נוספים עליך להוסיף אותם ל-`record`
        // FinishCallType = finishCallType;
    }
}
