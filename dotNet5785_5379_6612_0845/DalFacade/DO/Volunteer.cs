using DO.Enumes;

namespace DO
{
    public record Volunteer(
    int IdVolunteer,
    string Name = "",
    string PhoneNumber = "",
    string EmailOfVolunteer = "",
    string PasswordVolunteer = "",
    string AddressVolunteer = "",
    double VolunteerLatitude,
    double VolunteerLongitude,
    bool IsAvailable = false,
    double MaximumDistanceForReceivingCall,
    Enumes.Role Role,
    Enumes.DistanceType DistanceType = 0
     );
}

