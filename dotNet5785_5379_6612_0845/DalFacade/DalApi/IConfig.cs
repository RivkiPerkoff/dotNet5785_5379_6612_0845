

namespace DalApi;
using DO;
public interface IConfig
{
    public DateTime Clock { get; set; }
    public TimeSpan RiskRange { get; set; }
    //internal  void Reset();
    public int Create();
}

