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
    private static IDal s_dal = Factory.Get; //stage 4
    static public DateTime PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock)
    {
        return DateTime.Now;
    }
    internal static void ValidateVolunteer(Volunteer volunteer)
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
}
