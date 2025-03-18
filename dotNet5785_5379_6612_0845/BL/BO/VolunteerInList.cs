namespace BL.BO;

public class VolunteerInList
{
    public int VolunteerId { get; set; }
    public string Name { get; set; }
    public bool IsAvailable { get; set; }
    public int HandledCalls { get; set; }
    public int CanceledCalls { get; set; }
    public int ExpiredCalls { get; set; }
    public int? CurrentCallId { get; set; }
    public CallTypes CallType { get; set; }
    public override string ToString()
    {
        return $"VolunteerId: {VolunteerId}, " +
               $"Name: {Name}, " +
               $"IsAvailable: {IsAvailable}, " +
               $"HandledCalls: {HandledCalls}, " +
               $"CanceledCalls: {CanceledCalls}, " +
               $"ExpiredCalls: {ExpiredCalls}, " +
               $"CurrentCallId: {CurrentCallId}, " +
               $"CallType: {CallType}, ";
    }
}
