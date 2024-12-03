
using DalApi;

public interface IDal
{
    IVolunteer Volunteer { get; }
    IAssignment Assignment { get; }
    ICall Call { get; }
    void ResetDB();
}
