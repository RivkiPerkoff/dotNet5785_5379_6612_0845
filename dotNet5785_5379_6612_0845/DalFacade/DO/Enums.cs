namespace DO;
/// <summary>
/// Defines the types of distance used for calculating the range of a volunteer's work.
/// </summary>
public enum DistanceType
{
    AirDistance, 
    RoadDistance, 
    WalkingDistance
}

public enum Role
{
    Manager,
    Volunteer
}

public enum CallTypes

{
    MusicPerformance,
    MusicTherapy,
    SingingAndEmotionalSupport,
    GroupActivities,
    PersonalizedMusicCare
}
public enum FinishCallType
{
    TakenCareof,
    CanceledByVolunteer,
    CanceledByManager,
    Expired
}

public enum TypeOfReading
{
    Type1 = 1,
    Type2 = 2,
}

public enum TypeOfEndTime
{
    treated ,
    SelfCancellation,
    CancelingAnAdministrator,
    CancellationHasExpired
}

private enum MainMenuOptions
{
    Exit,
    VolunteerSubMenu,
    CallSubMenu,
    AssignmentSubMenu,
    ConfigurationSubMenu,
    InitializeData,
    DisplayAllData,
    ResetDatabase
}
public enum SubMenu
{
    Exit,
    Create,
    Read,
    ReadAll,
    UpDate,
    Delete,
    DeleteAll
}
private enum ConfigSubmenu
{
    Exit,
    AdvanceClockByMinute,
    AdvanceClockByHour,
    AdvanceClockByDay,
    AdvanceClockByMonth,
    AdvanceClockByYear,
    DisplayClock,
    ChangeClockOrRiskRange,
    DisplayConfigVar,
    Reset
}
