namespace DalApi;
/// <summary>
/// Interface for configuration settings in the data access layer.
/// Provides methods to manage system settings like IDs and time ranges.
/// </summary>
public interface IConfig
{
    public  DateTime Clock { get; set; }
    public TimeSpan? RiskRange { get; set; }
    public int CreateAssignmentId();
    public int CreateCallId();
    public int CreateVolunteerId();
    public void Reset();

}

