
namespace Dal;
using DO;
internal record Config
{
    internal const int StartVolunteerId = 1000;
    private static int s_nextVolunteerId = StartVolunteerId;
    public static int NextVolunteerId { get => s_nextVolunteerId++; }
    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static void Reset()
    {
        s_nextVolunteerId = StartVolunteerId;
        Clock = DateTime.Now;
    }

    internal static TimeSpan RiskRange { get;  set; }

    //internal static void SetRiskRange(TimeSpan? range)
    //{
    //    RiskRange = range;
    //}

    } 
