namespace DO;
/// <summary>
/// Represents a volunteer with personal details, availability, and preferences for call assignments.
/// </summary>
/// <param name="VolunteerId">A unique identifier for the volunteer.</param>
/// <param name="Name">The full name of the volunteer.</param>
/// <param name="PhoneNumber">The personal phone number of the volunteer.</param>
/// <param name="EmailOfVolunteer">The email address of the volunteer.</param>
/// <param name="PasswordVolunteer">The personal password for the volunteer (optional).</param>
/// <param name="AddressVolunteer">The full address of the volunteer.</param>
/// <param name="VolunteerLatitude">The latitude location of the volunteer (optional).</param>
/// <param name="VolunteerLongitude">The longitude location of the volunteer (optional).</param>
/// <param name="IsAvailable">Indicates if the volunteer is available to receive calls (default is false).</param>
/// <param name="MaximumDistanceForReceivingCall">The maximum distance from the volunteer within which they can receive a call (in kilometers).</param>
/// <param name="Role">The role of the volunteer, indicating whether they are a regular volunteer or a manager.</param>
/// <param name="DistanceType">The type of distance measurement used (e.g., air distance, walking distance, or road distance).</param>
public record Volunteer(
int VolunteerId,
string Name = "",
string PhoneNumber = "",
string EmailOfVolunteer = "",
string? PasswordVolunteer = "",
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
   

