
namespace Dal;
internal static class Config
{
    internal const int StartCourseId = 1000;
    private static int s_nextCourseId = StartCourseId;
    internal static int NextCourseId { get => s_nextCourseId++; }
    //...
    internal static DateTime Clock { get; } = DateTime.Now;
    //...
    internal static void Reset()
    {
       // s_nextCourseId = StartCourseId;
        //...
       // Clock = DateTime.Now;
        //...
    } 
}