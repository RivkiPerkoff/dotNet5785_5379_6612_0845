using BL.BIApi;
using BL.BO;
using BL.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BL.BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public string Login(string username, string password)
    {
        var volunteer = _dal.Volunteer.ReadAll()
            .FirstOrDefault(v => v.Name == username && v.PasswordVolunteer == password)
            ?? throw new BO.BlLoginFailedException("Username or password is incorrect");

        return ((Role)volunteer.Role).ToString();
    }

    public List<BO.VolunteerInList> GetVolunteers(bool? isActive, TypeSortingVolunteers? sortBy)
    {
        var volunteers = _dal.Volunteer.ReadAll().Select(v => new BO.VolunteerInList
        {
            VolunteerId = v.VolunteerId,
            Name = v.Name,
            IsAvailable = v.IsAvailable,
            //HandledCalls="",
            //CanceledCalls='',
            //ExpiredCalls='',
            //CurrentCallId='',
            // CallType=''
        });

        if (isActive.HasValue)
        {
            volunteers = volunteers.Where(v => v.IsAvailable == isActive.Value);
        }

        return sortBy switch
        {
            TypeSortingVolunteers.Name => volunteers.OrderBy(v => v.Name).ToList(),
            TypeSortingVolunteers.VolunteerId => volunteers.OrderBy(v => v.VolunteerId).ToList(),
            TypeSortingVolunteers.TotalCallsHandled => volunteers.OrderByDescending(v => v.HandledCalls).ToList(),
            _ => volunteers.OrderBy(v => v.VolunteerId).ToList(),
        };
    }
    public Volunteer GetVolunteerDetails(int volunteerId)
    {
        var doVolunteer = _dal.Volunteer.Read(volunteerId)
                         ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");

        return MapToBO(doVolunteer);
    }

    public void AddVolunteer(Volunteer volunteer)
    {
        ValidateVolunteer(volunteer);

        // Map BO to DO
        var doVolunteer = MapToDO(volunteer);

        try
        {
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={volunteer.VolunteerId} already exists", ex); // השתמשתי ב-VolunteerId
        }
    }

   


    public void DeleteVolunteer(int volunteerId)
    {
        var volunteer = _dal.Volunteer.Read(volunteerId)
                       ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");

        if (volunteer.TotalCallsHandled > 0 || volunteer.SelectedAndExpiredCalls > 0)
        {
            throw new BO.BlValidationException($"Volunteer with ID={volunteerId} cannot be deleted as they have handled calls");
        }

        try
        {
            _dal.Volunteer.Delete(volunteerId);
        }
        catch (DO.DalDoesNotExistException)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
        }
    }

    public void UpdateVolunteer(int requesterId, Volunteer volunteer)
    {
        var requester = _dal.Volunteer.Read(requesterId)
                        ?? throw new BO.BlDoesNotExistException($"Requester with ID={requesterId} does not exist");

        if (requester.Role != Role.Manager && requesterId != volunteer.VolunteerId) // עדכון ל-VolunteerId
        {
            throw new BO.BlUnauthorizedAccessException("Only admins or the volunteer themselves can update details");
        }

        ValidateVolunteer(volunteer);

        var existingVolunteer = _dal.Volunteer.Read(volunteer.VolunteerId) // עדכון ל-VolunteerId
                               ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteer.VolunteerId} does not exist");

        if (requester.Role != Role.Manager && existingVolunteer.Role != volunteer.Role)
        {
            throw new BO.BlUnauthorizedAccessException("Only admins can update the role");
        }

        var doVolunteer = MapToDO(volunteer);

        try
        {
            _dal.Volunteer.Update(doVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteer.VolunteerId} does not exist", ex); // עדכון ל-VolunteerId
        }
    }

    private void ValidateVolunteer(Volunteer volunteer)
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


    private Volunteer MapToBO(DO.Volunteer doVolunteer)
    {
        return new Volunteer
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
            Role = (Role)doVolunteer.Role, // המרה בין הטיפוסים
            DistanceType = (DistanceType)doVolunteer.DistanceType, // המרה בין הטיפוסים
            //TotalCallsHandled = doVolunteer.TotalCallsHandled,
            //TotalCallsCanceled = doVolunteer.TotalCallsCanceled,
            //SelectedAndExpiredCalls = doVolunteer.SelectedAndExpiredCalls,
            //CallInProgress = doVolunteer.callInProgress 
        };
    }

    private DO.Volunteer MapToDO(Volunteer volunteer)
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
}
