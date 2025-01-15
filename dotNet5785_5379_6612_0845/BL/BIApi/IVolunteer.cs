using BL.BO;

namespace BL.BIApi;

public interface IVolunteer
{
    void Login(string username, string password);
    public List<VolunteerInList> GetVolunteers(bool? isActive, TypeSortingVolunteers? TypeSortingVolunteers);


}
