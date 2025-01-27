//using DalApi;
namespace DalApi;
public interface IDal
{
    IVolunteer Volunteer { get; }
    IAssignment Assignment { get; }
    ICall Call { get; }
    IConfig Config { get; }
    void ResetDB();
}
