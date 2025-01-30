
using System;

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
    public override string ToString() => this.ToStringProperty();
}
