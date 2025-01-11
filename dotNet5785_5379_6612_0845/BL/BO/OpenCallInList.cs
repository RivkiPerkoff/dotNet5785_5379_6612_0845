namespace BL.BO;

public class OpenCallInList
{
    int Id { get; init; }
    CallTypes callTypes { get; set; }
    string? CallDescription { get; set; }
    public string Address { get; set; }
    DateTime OpeningTime { get; set; }
    DateTime? MaxFinishTime { get; set; }
    double calldistance { get; set; }
}

