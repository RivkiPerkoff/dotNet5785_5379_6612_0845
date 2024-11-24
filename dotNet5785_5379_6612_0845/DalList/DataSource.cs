

using DO;

namespace Dal;

internal static class DataSource
{
    public static IEnumerable<Volunteer> VolunteerName { get; internal set; }
    public static IEnumerable<Volunteer> Volunteers { get; set; }
    internal static List<DO.Volunteer> volunteers { get; } = new();
    public static IEnumerable<Call> Name { get; internal set; }
    internal static List<DO.Call> Calls { get; } = new();


}
