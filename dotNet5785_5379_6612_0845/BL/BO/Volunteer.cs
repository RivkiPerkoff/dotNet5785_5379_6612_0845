
namespace BL.BO;

public class Volunteer
{
    public int VolunteerId { get; init; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmailOfVolunteer { get; set; }
    public string? PasswordVolunteer { get; set; }
    public string? AddressVolunteer { get; set; }
    public double? VolunteerLatitude { get; set; }
    public double? VolunteerLongitude { get; set; }
    public bool IsAvailable { get; set; }/*IsActive*/
    public double? MaximumDistanceForReceivingCall { get; set; }
    public Role Role { get; set; }
    public DistanceType DistanceType { get; set; }
    public int TotalCallsHandled { get; set; }
    public int TotalCallsCanceled { get; set; }
    public int SelectedAndExpiredCalls { get; set; }
    public CallInProgress? CallInProgress { get; set; }
    public override string ToString()
    {
        return $"VolunteerId: {VolunteerId}, " +
               $"Name: {Name}, " +
               $"PhoneNumber: {PhoneNumber}, " +
               $"EmailOfVolunteer: {EmailOfVolunteer}, " +
               $"PasswordVolunteer: {PasswordVolunteer}, " +
               $"AddressVolunteer: {AddressVolunteer}, " +
               $"VolunteerLatitude: {VolunteerLatitude}, " +
               $"VolunteerLongitude: {VolunteerLongitude}, " +
               $"IsAvailable: {IsAvailable}, " +
               $"MaximumDistanceForReceivingCall: {MaximumDistanceForReceivingCall}, " +
               $"Role: {Role}, " +
               $"DistanceType: {DistanceType}, " +
               $"TotalCallsHandled: {TotalCallsHandled}, " +
               $"TotalCallsCanceled: {TotalCallsCanceled}, " +
               $"SelectedAndExpiredCalls: {SelectedAndExpiredCalls}, " +
               $"CallInProgress: {CallInProgress?.ToString() ?? "None"}";
    }
}
