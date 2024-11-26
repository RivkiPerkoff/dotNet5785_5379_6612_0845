

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
    public Assignment(int id) : this(0, 0, 0, DateTime.Now)
    {}
}