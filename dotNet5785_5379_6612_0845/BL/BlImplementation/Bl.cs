

using BL.BIApi;

namespace BL.BlImplementation;

internal class Bl : IBL
{
    public ICall Call { get; } = new CallImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public IAdmin Admin { get; } = new AdminImplementation();
}
