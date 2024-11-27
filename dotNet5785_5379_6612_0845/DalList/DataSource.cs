namespace Dal;
/// <summary>
/// Class that acts as a static data storage for system entities such as volunteers, calls, and assignments.
/// This class contains static lists that are used to store and retrieve the respective data in the system.
/// </summary>
internal static class DataSource
{
    public static List<DO.Volunteer> Volunteers { get; } = new();
    internal static List<DO.Call> Calls { get; } = new();
    internal static List<DO.Assignment> Assignments { get; } = new();
}
