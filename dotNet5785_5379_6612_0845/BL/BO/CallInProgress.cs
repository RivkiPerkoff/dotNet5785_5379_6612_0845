
namespace BL.BO;

internal class CallInProgress
{
    public int AllocationEntityId { get; init; }
    public int CallId { get; init; }
    CallTypes callTypes { get; init; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    double? VolunteerLatitude { get; set; }
    double? VolunteerLongitude { get; set; }
    Role Role { get; set; }
    double? MaxDistance { get; set; }
    public bool IsActive { get; set; }
    DistanceType DistanceType { get; set; }
    int TotalCallsHandled { get; set; }
    int TotalCallsCanceled { get; set; }
    int SelectedAndExpiredCalls { get; set; }
    CallInProgress? callInProgress { get; set; }
    public override string ToString() => this.ToStringProperty();
}
