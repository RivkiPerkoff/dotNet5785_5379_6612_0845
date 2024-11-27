namespace DO;
/// <summary>
/// Represents a call record, including information about the call's unique ID, description, location,
/// opening time, maximum finish time, and the type of reading associated with the call.
/// </summary>
/// <param name="IdCall">The unique ID for the call.</param>
/// <param name="CallDescription">A description of the call's purpose or details.</param>
/// <param name="CallAddress">The address where the call is taking place.</param>
/// <param name="CallLatitude">The latitude of the call location.</param>
/// <param name="CallLongitude">The longitude of the call location.</param>
/// <param name="OpeningTime">The time when the call starts.</param>
/// <param name="MaxFinishTime">The latest time by which the call should be finished.</param>
/// <param name="TypeOfReading">The type of reading associated with the call (e.g., Type1).</param>
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


