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
using Helpers;

namespace BL.Helpers;

/// <summary>
/// Manages volunteer-related operations such as validation, mapping, and authentication.
/// </summary>
static internal class VolunteerManager
{
    internal static ObserverManager Observers = new();
    private static IDal s_dal = Factory.Get;

    /// <summary>
    /// Updates periodic volunteer records based on time changes.
    /// </summary>
    /// <param name="oldClock">Previous clock timestamp.</param>
    /// <param name="newClock">New clock timestamp.</param>
    /// <returns>Updated DateTime value.</returns>
    //public static DateTime PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock)
    //{
    //    bool volunteerUpdated; //stage 5
    //                         //if a specific student info is changed - then call NotifyItemUpdated(id, false)
    //                         //and after all - call NotifyListUpdated();
    //    lock (AdminManager.blMutex) //stage 7
    //    {
    //        volunteerUpdated = false; //stage 5
    //        var list = s_dal.Volunteer.ReadAll().ToList(); //stage 4
    //        foreach (var doVolunteer in list) //stage 4
    //        {
    //            //if student study for more than MaxRange years
    //            //then student should be automatically updated to 'not active'
    //            if (AdminManager.Now.Year - doVolunteer.MaximumDistanceForReceivingCall?.Year >=
    //            s_dal.Config.RiskRange) //stage 4
    //            {
    //                volunteerUpdated = true; //stage 5
    //                s_dal.Volunteer.Update(doVolunteer with { IsAvailable= false }); //stage 4
    //                Observers.NotifyItemUpdated(doVolunteer.VolunteerId); //stage 5
    //            }
    //        }
    //    }
    //    //if the current year was changed
    //    //it means that we need to announce that the whole list of student was updated
    //    bool yearChanged = oldClock.Year != newClock.Year; //stage 5
    //    if (yearChanged || volunteerUpdated) //stage 5
    //        Observers.NotifyListUpdated(); //stage 5

    //    return DateTime.Now;
    //}

    /// <summary>
    /// Verifies if the input password matches the stored hashed password.
    /// </summary>
    /// <param name="inputPassword">User-provided password.</param>
    /// <param name="hashedPassword">Stored hashed password.</param>
    /// <returns>True if passwords match, otherwise false.</returns>
    public static bool VerifyPassword(string inputPassword, string hashedPassword)
    {
        return EncryptPassword(inputPassword) == hashedPassword;
    }

    /// <summary>
    /// Encrypts a password using SHA-256 hashing.
    /// </summary>
    /// <param name="password">Plain text password.</param>
    /// <returns>Hashed password string.</returns>
    public static string EncryptPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    /// <summary>
    /// Validates a volunteer's details, ensuring proper formatting and correctness.
    /// </summary>
    /// <param name="volunteer">Volunteer object to validate.</param>
    /// <exception cref="BO.BlValidationException">Thrown if validation fails.</exception>
    public static void ValidateVolunteer(BO.Volunteer volunteer)
    {
        if (string.IsNullOrWhiteSpace(volunteer.Name) || volunteer.Name.Length < 2)
        {
            throw new BO.BlValidationException("Name must be at least 2 characters long");
        }

        if (!string.IsNullOrWhiteSpace(volunteer.EmailOfVolunteer) &&
            !Regex.IsMatch(volunteer.EmailOfVolunteer, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new BO.BlValidationException("Invalid email format");
        }

        if (!string.IsNullOrWhiteSpace(volunteer.PhoneNumber) &&
            !Regex.IsMatch(volunteer.PhoneNumber, @"^\d{10}$"))
        {
            throw new BO.BlValidationException("Phone must be a valid 10-digit number");
        }

        if (!string.IsNullOrWhiteSpace(volunteer.AddressVolunteer))
        {
            try
            {
                (double latitude, double longitude) = Tools.GetCoordinatesFromAddress(volunteer.AddressVolunteer);
                volunteer.VolunteerLatitude = latitude;
                volunteer.VolunteerLongitude = longitude;
            }
            catch
            {
                throw new BO.BlValidationException("Invalid address - unable to find coordinates");
            }
        }
    }

    /// <summary>
    /// Maps a DO.Volunteer object to a BO.Volunteer object.
    /// </summary>
    /// <param name="doVolunteer">Data Object volunteer.</param>
    /// <returns>Business Object volunteer.</returns>
    public static BO.Volunteer MapToBO(DO.Volunteer doVolunteer)
    {
        return new BO.Volunteer
        {
            VolunteerId = doVolunteer.VolunteerId,
            Name = doVolunteer.Name,
            PhoneNumber = doVolunteer.PhoneNumber,
            EmailOfVolunteer = doVolunteer.EmailOfVolunteer,
            PasswordVolunteer = doVolunteer.PasswordVolunteer,
            AddressVolunteer = doVolunteer.AddressVolunteer,
            VolunteerLatitude = doVolunteer.VolunteerLatitude,
            VolunteerLongitude = doVolunteer.VolunteerLongitude,
            IsAvailable = doVolunteer.IsAvailable,
            MaximumDistanceForReceivingCall = doVolunteer.MaximumDistanceForReceivingCall,
            Role = (BO.Role)doVolunteer.Role,
            DistanceType = (BO.DistanceType)doVolunteer.DistanceType,
        };
    }

    /// <summary>
    /// Maps a BO.Volunteer object to a DO.Volunteer object.
    /// </summary>
    /// <param name="volunteer">Business Object volunteer.</param>
    /// <returns>Data Object volunteer.</returns>
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

    /// <summary>
    /// Converts a list of DO.Volunteer objects to a list of BO.Volunteer objects.
    /// </summary>
    /// <param name="volunteers">Collection of Data Object volunteers.</param>
    /// <returns>List of Business Object volunteers.</returns>
    public static List<BO.Volunteer> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
    {
        return volunteers.Select(MapToBO).ToList();
    }

    /// <summary>
    /// Maps a DO.Volunteer object to a simplified BO.VolunteerInList object.
    /// </summary>
    /// <param name="doVolunteer">Data Object volunteer.</param>
    /// <returns>Business Object volunteer summary.</returns>
    public static BO.VolunteerInList MapToVolunteerInList(DO.Volunteer doVolunteer)
    {
        return new BO.VolunteerInList
        {
            VolunteerId = doVolunteer.VolunteerId,
            Name = doVolunteer.Name,
            IsAvailable = doVolunteer.IsAvailable,
            HandledCalls = 0,
            CanceledCalls = 0,
            ExpiredCalls = 0,
            CurrentCallId = null,
            CallType = BO.CallTypes.None
        };
    }
}