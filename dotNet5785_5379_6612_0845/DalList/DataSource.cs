

using DalApi;
using DO;

namespace Dal;

internal static class DataSource
{
    public static List<DO.Volunteer> Volunteers { get; } = new();
    internal static List<DO.Call> Calls { get; } = new();
    internal static List<DO.Assignment> Assignments { get; } = new();
}
