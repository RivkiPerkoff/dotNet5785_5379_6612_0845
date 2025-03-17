using DalApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BL.BO;
using BL.Helpers;
using DO;
using BL.BIApi;
namespace BL.Helpers;
static internal class VolunteerManager
{
    private static IDal s_dal = Factory.Get;
    public static DateTime PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock)
    {
        return DateTime.Now;
    }
    public static void ValidateVolunteer(BO.Volunteer volunteer)
    {
        if (string.IsNullOrWhiteSpace(volunteer.Name) || volunteer.Name.Length < 2)
        {
            throw new BO.BlValidationException("Name must be at least 2 characters long");
        }

        if (!string.IsNullOrWhiteSpace(volunteer.EmailOfVolunteer) && !Regex.IsMatch(volunteer.EmailOfVolunteer, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new BO.BlValidationException("Invalid email format");
        }

        if (!string.IsNullOrWhiteSpace(volunteer.PhoneNumber) && !Regex.IsMatch(volunteer.PhoneNumber, @"^\d{10}$"))
        {
            throw new BO.BlValidationException("Phone must be a valid 10-digit number");
        }

        //if (!IdValidator.IsValid(volunteer.VolunteerId)) // עדכון ל-VolunteerId
        //{
        //    throw new BO.BlValidationException("Invalid ID - check digit is incorrect");
        //}

        if (!string.IsNullOrWhiteSpace(volunteer.AddressVolunteer)) // עדכון ל-AddressVolunteer
        {
            if (Tools.GetCoordinatesFromAddress(volunteer.AddressVolunteer) is (double latitude, double longitude))
            {
                volunteer.VolunteerLatitude = latitude;
                volunteer.VolunteerLongitude = longitude;
            }
            else
            {
                throw new BO.BlValidationException("Invalid address - unable to find coordinates");
            }
        }

    }

    public static BO.Volunteer MapToBO(DO.Volunteer doVolunteer)
    {
        return new BO.Volunteer
        {
            VolunteerId = doVolunteer.VolunteerId, // עדכון ל-VolunteerId
            Name = doVolunteer.Name,
            PhoneNumber = doVolunteer.PhoneNumber, // עדכון ל-PhoneNumber
            EmailOfVolunteer = doVolunteer.EmailOfVolunteer, // עדכון ל-EmailOfVolunteer
            PasswordVolunteer = doVolunteer.PasswordVolunteer, // עדכון ל-PasswordVolunteer
            AddressVolunteer = doVolunteer.AddressVolunteer, // עדכון ל-AddressVolunteer
            VolunteerLatitude = doVolunteer.VolunteerLatitude,
            VolunteerLongitude = doVolunteer.VolunteerLongitude,
            IsAvailable = doVolunteer.IsAvailable, // עדכון ל-IsAvailable
            MaximumDistanceForReceivingCall = doVolunteer.MaximumDistanceForReceivingCall, // עדכון ל-MaximumDistanceForReceivingCall
            Role = (BO.Role)doVolunteer.Role, // המרה בין הטיפוסים
            DistanceType = (BO.DistanceType)doVolunteer.DistanceType, // המרה בין הטיפוסים
            //TotalCallsHandled = 0,
            //TotalCallsCanceled = 0,
            //SelectedAndExpiredCalls = 0,
            //CallInProgress = null
        };
    }
    public static DO.Volunteer MapToDO(BO.Volunteer volunteer)
    {
        return new DO.Volunteer(
               volunteer.VolunteerId,
               volunteer.Name,
               volunteer.PhoneNumber,
               volunteer.EmailOfVolunteer,
               volunteer.PasswordVolunteer,
               volunteer.AddressVolunteer,
               volunteer.VolunteerLatitude,
               volunteer.VolunteerLongitude,
               volunteer.IsAvailable,
               volunteer.MaximumDistanceForReceivingCall,
               (DO.Role)volunteer.Role
               );
    }
 
    public static List<BO.Volunteer> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
    {
        return volunteers.Select(MapToBO).ToList();
    }
    public static BO.VolunteerInList MapToVolunteerInList(DO.Volunteer doVolunteer)
    {
        return new BO.VolunteerInList
        {
            VolunteerId = doVolunteer.VolunteerId,
            Name = doVolunteer.Name,
            IsAvailable = doVolunteer.IsAvailable,
            HandledCalls = 0, // ערכים ברירת מחדל או לחשב מתוך הנתונים
            CanceledCalls = 0,
            ExpiredCalls = 0,
            CurrentCallId = null,
            CallType =BO.CallTypes.None
        };
    }


}