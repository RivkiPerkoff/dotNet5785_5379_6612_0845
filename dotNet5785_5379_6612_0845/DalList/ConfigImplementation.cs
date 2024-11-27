namespace Dal;
using DalApi;
//using DO;

public class ConfigImplementation : IConfig
{
    public  DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    public TimeSpan? RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }
    public int CreateVolunteerId()
    {
        return Config.NextVolunteerId;
    }

    public int CreateCallId()
    {
        return Config.NextCallId;
    }

    public int CreateAssignmentId()
    {
        return Config.NextAssignmentId;
    }
    public void Reset()
    {
        Config.Reset();
    }

}
