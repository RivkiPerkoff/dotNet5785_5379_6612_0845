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
        return $"Volunteer ID: {VolunteerId}, Name: {Name}, " +
               $"Available: {(IsAvailable ? "Yes" : "No")}, " +
               $"Handled Calls: {HandledCalls}, Canceled Calls: {CanceledCalls}, " +
               $"Expired Calls: {ExpiredCalls}, " +
               $"Current Call ID: {(CurrentCallId.HasValue ? CurrentCallId.Value.ToString() : "N/A")}, " +
               $"Call Type: {CallType}";
    }

}
