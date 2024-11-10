
namespace Dal;

internal static class Config
{
    internal const int StartVolunteerId = 1000;
    private static int s_nextVolunteerId = StartVolunteerId;
    internal static int NextVolunteerId { get => s_nextVolunteerId++; }
    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static void Reset()
    {
        s_nextVolunteerId = StartVolunteerId;
        Clock = DateTime.Now;
        ////
    } 
}