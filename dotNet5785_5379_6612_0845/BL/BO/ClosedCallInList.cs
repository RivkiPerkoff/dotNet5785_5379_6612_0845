using DO;

namespace BL.BO;

public class ClosedCallInList
{
    int Id { get; init; }
    CallTypes callTypes { get; set; }
    string Address { get; set; }
    DateTime OpeningTime { get; set; }
    DateTime EntryTimeForTreatment { get; set; }
    DateTime? EndTimeForTreatment { get; set; }
    TypeOfEndTime? TypeOfEndTime { get; set; }

}

