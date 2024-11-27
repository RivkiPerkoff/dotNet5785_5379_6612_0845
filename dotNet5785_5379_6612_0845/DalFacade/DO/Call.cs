namespace DO;

public record Call
(
    int IdCall = 0,
    string? CallDescription = "",
    string? CallAddress = "",
    double CallLatitude = 0,
    double CallLongitude = 0,
    DateTime? OpeningTime = null,
    DateTime? MaxFinishTime = null,
    TypeOfReading TypeOfReading = TypeOfReading.Type1
)
{
    // קונסטרקטור מותאם אישית
    public Call(int idCall, string? callDescription, string? callAddress, double callLatitude, double callLongitude, DateTime openingTime, DateTime maxFinishTime, TypeOfReading typeOfReading)
        : this(idCall, callDescription, callAddress, callLatitude, callLongitude, (DateTime?)openingTime, (DateTime?)maxFinishTime, typeOfReading)
    {
    }
}


