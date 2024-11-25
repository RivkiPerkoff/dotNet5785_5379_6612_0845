
namespace Dal;
using DO;
internal record Config
{
    internal const int StartVolunteerId = 1000;

    private static int s_nextVolunteerId = StartVolunteerId;
    public static int NextVolunteerId => s_nextVolunteerId++;

    public static DateTime Clock ;
    internal static TimeSpan RiskRange { get; set; }
    internal void Reset()
    {
        s_nextVolunteerId = StartVolunteerId;
        Clock = DateTime.Now;
    }
}
