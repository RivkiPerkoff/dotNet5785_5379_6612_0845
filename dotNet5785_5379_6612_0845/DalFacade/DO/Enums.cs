namespace DO;
public enum FinishCallType
{
    TakenCareof,
    CanceledByVolunteer,
    CanceledByManager,
    Expired
}
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
    None,
    ManDriver,
    WomanDriver

}
public enum MainMenuOptions
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
public enum ConfigSubmenu
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
