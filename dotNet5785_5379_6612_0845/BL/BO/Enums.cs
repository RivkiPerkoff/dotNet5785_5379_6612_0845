
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
    open,
    Pending
}
/// <summary>
/// Enum representing the fields of the CallInList class.
/// </summary>
public enum CallInListFields
{
    Id,
    CallId,
    CallType,
    StartTime,
    TimeToEnd,
    LastUpdateBy,
    TimeToCompleteTreatment,
    Status,
    TotalAssignment
}
/// <summary>
/// Enum representing the fields in the ClosedCallInList class.
/// </summary>
public enum ClosedCallInListFields
{
    Id,
    CallTypes,
    Address,
    OpeningTime,
    EntryTimeForTreatment,
    EndTimeForTreatment,
    FinishCallType
}

public enum OpenCallInListFields
{
    Id,
    CallTypes,
    CallDescription,
    Address,
    OpeningTime,
    MaxFinishTime,
    Calldistance,
}
public enum CallTypes
{
    None,
    ManDriver,
    WomanDriver
    
}
public enum TreatmentEndType
{

}

public enum VolunteerFields
{
    All,
    VolunteerId,
    Name,
    PhoneNumber,
    EmailOfVolunteer,
    PasswordVolunteer,
    AddressVolunteer,
    VolunteerLatitude,
    VolunteerLongitude,
    IsAvailable,
    MaximumDistanceForReceivingCall,
    Role,
    DistanceType,
    TotalCallsHandled,
    TotalCallsCanceled,
    SelectedAndExpiredCalls,
    CallInProgress
}
