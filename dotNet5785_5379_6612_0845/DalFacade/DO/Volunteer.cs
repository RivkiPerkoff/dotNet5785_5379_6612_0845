//using DO.Enumes;

namespace DO
{
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
    double? MaximumDistanceForReceivingCall = 0
    //Enumes.Role Role ,
    //Enumes.DistanceType DistanceType 
     );
//    {
//    public Volunteer():this()
//}
}

