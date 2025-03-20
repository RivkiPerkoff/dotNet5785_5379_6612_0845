using DO;
namespace BL.BO;

public class OpenCallInList
{
   public int Id { get; init; }
   public DO.CallTypes CallTypes { get; set; }
   public string? CallDescription { get; set; }
   public  string Address { get; set; }
   public DateTime OpeningTime { get; set; }
   public DateTime? MaxFinishTime { get; set; }
   public double CallDistance { get; set; }
    public override string ToString()
    {
        return $"Id: {Id}, Call Type: {CallTypes}, " +
               $"Description: {(string.IsNullOrEmpty(CallDescription) ? "N/A" : CallDescription)}, " +
               $"Address: {Address}, Opening Time: {OpeningTime}, " +
               $"Max Finish Time: {(MaxFinishTime.HasValue ? MaxFinishTime.Value.ToString() : "N/A")}, " +
               $"Call Distance: {CallDistance:F2} km";
    }

}

