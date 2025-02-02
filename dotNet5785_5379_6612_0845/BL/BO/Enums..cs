
namespace BL.BO;
public enum Role
{
    Manager,
    Volunteer
}
public enum DistanceType
{
    AirDistance,
    RoadDistance,
    WalkingDistance
}
public enum CallTypes
{
    ManDriver,
    WomanDriver,
    None
}
public enum RiskRangeStatus
{
    InTreatment,
    InRiskTreatment
}
public enum TypeSortingVolunteers
{

    VolunteerId,
    Name,
    IsAvailable,
    HandledCalls,
    CanceledCalls,
    ExpiredCalls,
    CurrentCallId,
    CallType
}

public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}

public enum StatusCallType
{
    HandlingInRisk,
    inHandling,
    closed,
    Expired,
    openInRisk,
    open
}
public enum TreatmentEndType
{

}
public enum CallInListFields
{
    Id,
    CallId,
    CallType,
    StartTime,
    TimeToEnd,
    LastUpdateBy,
    TimeTocompleteTreatment,
    Status,
    TotalAssignment
}
