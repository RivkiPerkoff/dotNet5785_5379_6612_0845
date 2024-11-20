

using DO;

namespace Dal;

internal static class DataSource
{
    public static IEnumerable<Volunteer> Name { get; internal set; }
    public static IEnumerable<Volunteer> Volunteers { get;  set; }
    internal static List<DO.Volunteer> volunteers { get; } = new();

}
