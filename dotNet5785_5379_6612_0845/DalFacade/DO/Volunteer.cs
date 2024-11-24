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
    double? MaximumDistanceForReceivingCall = 0,
    //Enumes.Role Role ,
    Enumes.DistanceType DistanceType= Enumes.DistanceType.AirDistance
     )
    {
        public Volunteer(int volunteerId, string name, string emailOfVolunteer, string phoneNumber, string addressVolunteer)
            : this(volunteerId, name, phoneNumber, emailOfVolunteer, "", addressVolunteer, 0, 0, false, 0, Enumes.DistanceType.AirDistance)
        {
        }

        // קונסטרקטור ברירת מחדל
        public Volunteer() : this(0, "", "", "", "", "", 0, 0, false, 0, Enumes.DistanceType.AirDistance) { }

    }
    //    {
    //    public Volunteer():this()
    //    }
}       

