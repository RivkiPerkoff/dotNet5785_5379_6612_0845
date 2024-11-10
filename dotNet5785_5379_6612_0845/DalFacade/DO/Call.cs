

namespace DO
{
    internal class Call
    {
        int IdCall;
        string CallDescription = null;
        string CallAddress = "";
        double CallLatitude;
        double CallLongitude;
        internal static DateTime OpeningTime { get; set; } = DateTime.Now;
    }
}
