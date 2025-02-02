using DalApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BL.BO
namespace BL.Helpers;

static internal class VolunteerManager
{
    private static IDal s_dal = Factory.Get; 
     public static DateTime PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock)
    {
        return DateTime.Now;
    }
    private static void ValidateVolunteer(BO.Volunteer volunteer)
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

        if (!IdValidator.IsValid(volunteer.VolunteerId)) // עדכון ל-VolunteerId
        {
            throw new BO.BlValidationException("Invalid ID - check digit is incorrect");
        }

        if (!string.IsNullOrWhiteSpace(volunteer.AddressVolunteer)) // עדכון ל-AddressVolunteer
        {
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(volunteer.AddressVolunteer) ?? throw new BO.BlValidationException("Invalid address - unable to find coordinates");

            volunteer.VolunteerLatitude = latitude;
            volunteer.VolunteerLongitude = longitude;
        }
    }


    private static BO.Volunteer MapToBO(DO.Volunteer doVolunteer)
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
            //TotalCallsHandled = doVolunteer.TotalCallsHandled,
            //TotalCallsCanceled = doVolunteer.TotalCallsCanceled,
            //SelectedAndExpiredCalls = doVolunteer.SelectedAndExpiredCalls,
            //CallInProgress = doVolunteer.callInProgress 
        };
    }

    private static DO.Volunteer MapToDO(BO.Volunteer volunteer)
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
            (DO.Role)volunteer.Role,
            (DO.DistanceType)volunteer.DistanceType
        );

    }

        if (!string.IsNullOrWhiteSpace(volunteer.PhoneNumber) && !Regex.IsMatch(volunteer.PhoneNumber, @"^\d{10}$"))
        {
            throw new BO.BlValidationException("Phone must be a valid 10-digit number");
        }

        if (!IdValidator.IsValid(volunteer.VolunteerId)) // עדכון ל-VolunteerId
        {
            throw new BO.BlValidationException("Invalid ID - check digit is incorrect");
        }

        if (!string.IsNullOrWhiteSpace(volunteer.AddressVolunteer)) // עדכון ל-AddressVolunteer
        {
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(volunteer.AddressVolunteer) ?? throw new BO.BlValidationException("Invalid address - unable to find coordinates");

            volunteer.VolunteerLatitude = latitude;
            volunteer.VolunteerLongitude = longitude;
        }
    }
}
