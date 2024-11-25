

namespace DO;

public record Call
{
    public int IdCall { get; init; }
    public string? CallDescription { get; init; }
    public string? CallAddress { get; init; }
    public double? CallLatitude { get; init; }
    public double? CallLongitude { get; init; }
    public DateTime OpeningTime { get; init; }

    enum CallTypes;

    public Call(int idCall, string? callDescription, string? callAddress, double? callLatitude, double? callLongitude, DateTime openingTime)
    {
        IdCall = idCall;
        CallDescription = callDescription;
        CallAddress = callAddress;
        CallLatitude = callLatitude;
        CallLongitude = callLongitude;
        OpeningTime = openingTime;
    }
}
