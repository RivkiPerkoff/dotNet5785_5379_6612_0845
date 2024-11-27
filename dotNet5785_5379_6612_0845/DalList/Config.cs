namespace Dal;
using DO;
/// <summary>
/// Configuration class that manages the generation of unique IDs for volunteers, calls, and assignments,
/// as well as the system clock and risk range parameters.
/// </summary>
internal record Config
{
    /// <summary>
    /// The starting ID for volunteers.
    /// </summary>
    internal const int StartVolunteerId = 1000;
    private static int s_nextVolunteerId = StartVolunteerId;

    /// <summary>
    /// Gets the next unique volunteer ID and increments the ID for the next call.
    /// </summary>
    public static int NextVolunteerId => s_nextVolunteerId++;

    /// <summary>
    /// The starting ID for calls.
    /// </summary>
    internal const int StartCallId = 1000;
    private static int s_nextCallId = StartCallId;

    /// <summary>
    /// Gets the next unique call ID and increments the ID for the next call.
    /// </summary>
    public static int NextCallId => s_nextCallId++;

    /// <summary>
    /// The starting ID for assignments.
    /// </summary>
    internal const int StartAssignmentId = 1000;
    private static int s_nextAssignmentId = StartAssignmentId;

    /// <summary>
    /// Gets the next unique assignment ID and increments the ID for the next assignment.
    /// </summary>
    public static int NextAssignmentId => s_nextAssignmentId++;

    /// <summary>
    /// The system clock, initialized to the current date and time.
    /// </summary>
    public static DateTime Clock { get; internal set; } = DateTime.Now;

    /// <summary>
    /// Resets the IDs for volunteers, calls, and assignments to their starting values, and sets the clock to the current date and time.
    /// </summary>
    public static void Reset()
    {
        s_nextVolunteerId = StartVolunteerId;
        s_nextCallId = StartCallId;
        s_nextAssignmentId = StartAssignmentId;
        Clock = DateTime.Now;
    }

    /// <summary>
    /// The time range considered for risk, initialized to 1 hour and 30 minutes.
    /// </summary>
    internal static TimeSpan? RiskRange { get; set; } = new TimeSpan(1, 30, 0);
}
