

namespace DO;
/// <summary>
/// 
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
    private DateTime randomTime;
    private DateTime dateTime;
    private FinishCallType finishCallType;

    public Assignment(int id) : this(0, 0, 0, DateTime.Now)
    {}

    public Assignment(DateTime randomTime, DateTime dateTime, FinishCallType finishCallType)
    {
        this.randomTime = randomTime;
        this.dateTime = dateTime;
        this.finishCallType = finishCallType;
    }
}