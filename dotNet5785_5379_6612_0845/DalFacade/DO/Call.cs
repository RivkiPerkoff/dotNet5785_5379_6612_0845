

namespace DO;

internal record Call
{
    int IdCall;
    string? CallDescription = null;
    string? CallAddress = null;
    double? CallLatitude=0;
    double? CallLongitude=0;
    internal static DateTime OpeningTime { get; set; } = DateTime.Now;
}
