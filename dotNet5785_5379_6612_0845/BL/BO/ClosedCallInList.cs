using DO;

namespace BL.BO;

public class ClosedCallInList
{
  public int Id { get; init; }
  public DO.CallTypes CallTypes { get; set; }
  public string Address { get; set; }
  public DateTime OpeningTime { get; set; }
  public DateTime EntryTimeForTreatment { get; set; }
  public DateTime? EndTimeForTreatment { get; set; }
  public FinishCallType? FinishCallType { get; set; }

}

