


namespace Dal;
using DalApi;
using System;

public class ConfigImplementation : IConfig

{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskRange 
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException(); 
    }

    public void Reset()
    {
        Config.Reset();
    }


}
