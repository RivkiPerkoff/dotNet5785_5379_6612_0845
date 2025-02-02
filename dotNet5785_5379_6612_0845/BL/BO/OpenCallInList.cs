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
}

