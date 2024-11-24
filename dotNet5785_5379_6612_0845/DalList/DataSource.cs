

using DO;

namespace Dal;

internal static class DataSource
{
    public static List<Volunteer> Volunteers { get; } = new List<Volunteer>();
    internal static List<DO.Call> Calls { get; } = new();
    internal static List<DO.Assignment> Assignments { get; } = new();
}
