using DO;

namespace BL.BO;

public class ClosedCallInList
{
  public int Id { get; init; }
    public CallTypes CallTypes { get; set; }
  public string Address { get; set; }
  public DateTime OpeningTime { get; set; }
  public DateTime EntryTimeForTreatment { get; set; }
  public DateTime? EndTimeForTreatment { get; set; }
  public TreatmentEndType? FinishCallType { get; set; }
    public override string ToString()
    {


        return $"Id: {Id}, Call Type: {CallTypes}, Address: {Address}, " +
               $"Opening Time: {OpeningTime}, Entry Time: {EntryTimeForTreatment}, " +
               $"End Time: {(EndTimeForTreatment.HasValue ? EndTimeForTreatment.Value.ToString() : "N/A")}, " +
               $"Finish Call Type: {(FinishCallType.HasValue ? FinishCallType.Value.ToString() : "N/A")}";
    }


}

