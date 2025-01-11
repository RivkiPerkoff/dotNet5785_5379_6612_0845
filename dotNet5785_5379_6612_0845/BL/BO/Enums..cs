

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
    WomanDriver
}

public enum StatusCallType
{
    Open,
    InProgress,
    Closed,
    OpenAtRisk,
    InRiskManagement,
    Expired
}
public enum TreatmentEndType
{
    treated,
    SelfCancellation,
    CancelingAnAdministrator,
    CancellationHasExpired
}