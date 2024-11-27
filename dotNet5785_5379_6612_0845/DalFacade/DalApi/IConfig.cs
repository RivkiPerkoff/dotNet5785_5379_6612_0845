

namespace DalApi;
using DO;
public interface IConfig
{
    public  DateTime Clock { get; set; }
    public TimeSpan RiskRange { get; set; }
    public int CreateAssignmentId();
    public int CreateCallId();
    public int CreateVolunteerId();
    public void Reset();

}

