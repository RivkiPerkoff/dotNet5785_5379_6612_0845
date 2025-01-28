using BL.BIApi;
using BL.BO;

namespace BL.BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    public void AddVolunteer(Volunteer volunteer)
    {
        throw new NotImplementedException();
    }

    public void DeleteVolunteer(int volunteerId)
    {
        throw new NotImplementedException();
    }

    public Volunteer GetVolunteerDetails(int volunteerId)
    {
        throw new NotImplementedException();
    }

    public List<VolunteerInList> GetVolunteers(bool? isActive, TypeSortingVolunteers? sortBy)
    {
        throw new NotImplementedException();
    }

    public string Login(string username, string password)
    {
        throw new NotImplementedException();
    }

    public void UpdateVolunteer(int requesterId, Volunteer volunteer)
    {
        throw new NotImplementedException();
    }

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

}
