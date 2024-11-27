
namespace Dal;
using DO;
internal record Config
{
    internal const int StartVolunteerId = 1000;
    private static int s_nextVolunteerId = StartVolunteerId;

    public static int NextVolunteerId => s_nextVolunteerId++;

    internal const int StartCallId = 1000;
    private static int s_nextCallId = StartCallId;

    public static int NextCallId => s_nextCallId++;

    internal const int StartAssignmentId = 1000;
    private static int s_nextAssignmentId = StartAssignmentId;
    public static int NextAssignmentId => s_nextAssignmentId++;

    public static DateTime Clock { get;internal set; } = DateTime.Now;

    public static void Reset()
    {
        s_nextVolunteerId = StartVolunteerId;
        s_nextCallId=StartCallId;
        s_nextAssignmentId=StartAssignmentId;   
        Clock = DateTime.Now;
    }

    internal static TimeSpan? RiskRange { get; set; } = new TimeSpan(1, 30, 0);
}
