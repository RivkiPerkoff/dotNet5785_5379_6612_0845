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
static public class VolunteerManager
{
    internal static ObserverManager Observers = new();
    private static IDal s_dal = Factory.Get;

    /// <summary>
    /// Updates periodic volunteer records based on time changes.
    /// </summary>
    /// <param name="oldClock">Previous clock timestamp.</param>
    /// <param name="newClock">New clock timestamp.</param>
    /// <returns>Updated DateTime value.</returns>
    
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

    public static void ValidateVolunteer(BO.Volunteer volunteer)
    {
        try
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
        catch (BO.BlValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlValidationException($"Unexpected validation error: {ex.Message}", ex);
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
               (DO.Role)volunteer.Role,
               (DO.DistanceType)volunteer.DistanceType
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
            HandledCalls = CallManager.GetCountOfCompletedCalls(doVolunteer.VolunteerId),
            CanceledCalls = CallManager.GetCountOfSelfCancelledCalls(doVolunteer.VolunteerId),
            ExpiredCalls = CallManager.GetCountOfExpiredCalls(doVolunteer.VolunteerId),
            CurrentCallId = CallManager.GetCallInTreatment(doVolunteer.VolunteerId),
            CallType = BO.CallTypes.None
        };
    }
    private static int s_periodicCounter = 0;

    internal static void PeriodicVolunteerUpdates(DateTime oldClock, DateTime newClock)
    {
        Thread.CurrentThread.Name = $"PeriodicVolunteer{++s_periodicCounter}";

        bool shouldNotifyList = false;
        List<int> volunteersToNotify = new();

        List<DO.Volunteer> volunteerList;
        List<DO.Assignment> assignmentList;
        List<DO.Call> callList;

        // שלב 1: שליפת נתונים ב־lock עם המרה ל־List
        lock (AdminManager.BlMutex)
        {
            volunteerList = s_dal.Volunteer.ReadAll().ToList();
            assignmentList = s_dal.Assignment.ReadAll(a => a.EndTimeForTreatment == null).ToList();
            callList = s_dal.Call.ReadAll().ToList();
        }

        // שלב 2: טיפול במתנדבים עם קריאות שפגו
        foreach (var volunteer in volunteerList)
        {
            var assignment = assignmentList.FirstOrDefault(a => a.VolunteerId == volunteer.VolunteerId);

            if (assignment != null)
            {
                var call = callList.FirstOrDefault(c => c.IdCall == assignment.IdOfRunnerCall);

                if (call != null && call.MaxFinishTime.HasValue && call.MaxFinishTime.Value <= newClock)
                {
                    lock (AdminManager.BlMutex)
                    {
                        s_dal.Assignment.Update(assignment with
                        {
                            EndTimeForTreatment = AdminManager.Now,
                            FinishCallType = DO.FinishCallType.Expired
                        });
                        s_dal.Volunteer.Update(volunteer with { IsAvailable = true });
                    }

                    volunteersToNotify.Add(volunteer.VolunteerId);
                }
            }
        }

        // שלב 3: בדיקת שינוי שנה או עדכון מתנדבים
        bool yearChanged = oldClock.Year != newClock.Year;
        if (yearChanged || volunteersToNotify.Any())
            shouldNotifyList = true;

        // שלב 4: שליחת התראות מחוץ ל־lock
        foreach (var id in volunteersToNotify.Distinct())
            VolunteerManager.Observers.NotifyItemUpdated(id);

        if (shouldNotifyList)
            VolunteerManager.Observers.NotifyListUpdated();
    }


}