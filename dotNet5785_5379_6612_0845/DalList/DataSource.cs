

namespace Dal;

internal static class DataSource
{
    public static object Volunteers { get; internal set; }
    internal static List<DO.Volunteer> volunteers { get; } = new();

}
