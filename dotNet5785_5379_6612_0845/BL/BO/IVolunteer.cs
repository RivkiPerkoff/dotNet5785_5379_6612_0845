

namespace BL.BO;

public interface IVolunteer
{
    void Login(string username, string password);
    public List<VolunteerInList> GetVolunteers(Boolean? isActive, TypeSortingVolunteers? TypeSortingVolunteers);


}
