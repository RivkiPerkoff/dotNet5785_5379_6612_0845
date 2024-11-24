namespace Dal;
using DalApi;
using DO;

public class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    public void Reset()
    {
        Config.Reset();
    }

    // מימוש הפונקציה Create
    public int Create()
    {
        return Config.NextVolunteerId; // מחזיר ID רץ ייחודי
    }
}
