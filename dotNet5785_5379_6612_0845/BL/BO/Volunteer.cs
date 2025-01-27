
using System;

namespace BL.BO;

public class Volunteer
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    public double? VolunteerLatitude { get; set; }
    public double? VolunteerLongitude { get; set; }
    public Role Role { get; set; }
    public double? MaxDistance { get; set; }
    public bool IsActive { get; set; }
    public DistanceType DistanceType { get; set; }
    public int TotalCallsHandled { get; set; }
    public int TotalCallsCanceled { get; set; }
    public int SelectedAndExpiredCalls { get; set; }
    public CallInProgress? callInProgress { get; set; }
    public override string ToString() => this.ToStringProperty();
}
