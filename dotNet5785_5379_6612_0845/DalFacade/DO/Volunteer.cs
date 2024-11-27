
namespace DO;

public record Volunteer(
int VolunteerId,
string Name = "",
string PhoneNumber = "",
string EmailOfVolunteer = "",
string PasswordVolunteer = "",
string AddressVolunteer = "",
double? VolunteerLatitude = 0,
double? VolunteerLongitude = 0,
bool IsAvailable = false,
double? MaximumDistanceForReceivingCall = 0,
Role Role = Role.Volunteer,
DistanceType DistanceType= DistanceType.AirDistance
 )
{
    public Volunteer(int volunteerId, string name, string emailOfVolunteer, string phoneNumber, string addressVolunteer)
        : this(volunteerId, name, phoneNumber, emailOfVolunteer, "", addressVolunteer, 0, 0, false, 0, 0)
    {}
}
   

