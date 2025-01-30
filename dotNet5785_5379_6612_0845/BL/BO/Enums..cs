﻿

using System;

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
    Name,
    VolunteerId,
    TotalCallsHandled
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

}
