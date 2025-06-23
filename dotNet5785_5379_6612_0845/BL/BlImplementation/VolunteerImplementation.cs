using BL.BIApi;
using BL.BO;
using BL.Helpers;
using DO;
using System.Collections.Generic;
using System.Linq;

namespace BL.BlImplementation;

/// <summary>
/// Implements the IVolunteer interface to manage volunteer-related operations such as login, details retrieval, adding, updating, deleting, and listing volunteers.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Allows a volunteer to log in by verifying their username and password.
    /// </summary>
    /// <param name="username">The volunteer's username.</param>
    /// <param name="password">The volunteer's password.</param>
    /// <returns>The role of the volunteer.</returns>
    /// <exception cref="BlInvalidOperationException">Thrown when the username or password is incorrect.</exception>
    //public string Login(string username, string password)
    //{
    //    var volunteer = _dal.Volunteer.ReadAll()
    //            .FirstOrDefault(v => v.Name == username && v.PasswordVolunteer == password)
    //         ?? throw new BlInvalidOperationException("Username or password is incorrect");

    //    return (volunteer.Role).ToString();
    //}
    public string Login(string idString, string password)
    {
        if (!int.TryParse(idString, out int id))
            throw new BlInvalidOperationException("Invalid ID format");

        var volunteer = _dal.Volunteer.ReadAll()
            .FirstOrDefault(v => v.VolunteerId == id && v.PasswordVolunteer == password)
         ?? throw new BlInvalidOperationException("Username or password is incorrect");

        return (volunteer.Role).ToString();
    }


    /// <summary>
    /// Retrieves a list of volunteers, optionally filtered by availability and sorted based on specified parameters.
    /// </summary>
    /// <param name="isActive">If specified, filters the volunteers based on their availability status.</param>
    /// <param name="sortBy">The sorting parameter for volunteers.</param>
    /// <returns>A list of volunteers.</returns>
    /// <exception cref="BlGeneralDatabaseException">Thrown if there is an error accessing data or an unexpected error occurs.</exception>
    public List<BO.VolunteerInList> GetVolunteers(bool? isActive, TypeSortingVolunteers? sortBy)
    {
        try
        {
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v =>
            {
                if (!isActive.HasValue || v.IsAvailable == isActive.Value)
                {
                    return true;
                }
                return false;
            });

            List<BO.VolunteerInList> volunteerList = new List<BO.VolunteerInList>();

            foreach (DO.Volunteer volunteer in volunteers)
            {
                BO.VolunteerInList volunteerInList = VolunteerManager.MapToVolunteerInList(volunteer);
                volunteerList.Add(volunteerInList);
            }

            volunteerList = sortBy.HasValue ? sortBy.Value switch
            {
                BO.TypeSortingVolunteers.VolunteerId => volunteerList.OrderBy(v => v.VolunteerId).ToList(),
                BO.TypeSortingVolunteers.Name => volunteerList.OrderBy(v => v.Name).ToList(),
                BO.TypeSortingVolunteers.IsAvailable => volunteerList.OrderBy(v => v.IsAvailable).ToList(),
                BO.TypeSortingVolunteers.HandledCalls => volunteerList.OrderByDescending(v => v.HandledCalls).ToList(),
                BO.TypeSortingVolunteers.CanceledCalls => volunteerList.OrderByDescending(v => v.CanceledCalls).ToList(),
                BO.TypeSortingVolunteers.ExpiredCalls => volunteerList.OrderByDescending(v => v.ExpiredCalls).ToList(),
                BO.TypeSortingVolunteers.CurrentCallId => volunteerList.OrderBy(v => v.CurrentCallId).ToList(),
                BO.TypeSortingVolunteers.CallType => volunteerList.OrderBy(v => v.CallType).ToList(),
                _ => volunteerList.OrderBy(v => v.VolunteerId).ToList()
            } : volunteerList.OrderBy(v => v.VolunteerId).ToList();

            return volunteerList;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlGeneralDatabaseException("Error accessing data.", ex);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("An unexpected error occurred while getting Volunteers.", ex);
        }
    }

    /// <summary>
    /// Retrieves detailed information for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The volunteer's ID.</param>
    /// <returns>The volunteer details, including assignments and current call information.</returns>
    /// <exception cref="BlDoesNotExistException">Thrown when the volunteer does not exist.</exception>
    /// <exception cref="BlGeneralDatabaseException">Thrown if there is an error retrieving volunteer details.</exception>
    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            var doVolunteer = _dal.Volunteer.Read(volunteerId) ??
               throw new DO.DalDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
            var volunteer = VolunteerManager.MapToBO(doVolunteer);
            var assigments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
            var currentAssignment = assigments.FirstOrDefault(a => a.EndTimeForTreatment == null);
            BO.CallInProgress? callInProgress = null;
            if (currentAssignment != null)
            {
                var callDetails = _dal.Call.Read(currentAssignment.IdOfRunnerCall);
                if (callDetails != null)
                {
                    callInProgress = new BO.CallInProgress
                    {
                        Id = currentAssignment.NextAssignmentId,
                        CallId = currentAssignment.IdOfRunnerCall,
                        CallTypes = (BO.CallTypes)callDetails.CallTypes,
                        Description = callDetails.CallDescription,
                        CallingAddress = callDetails.CallAddress!,
                        OpeningTime = (DateTime)callDetails.OpeningTime,
                        MaxFinishTime = callDetails.MaxFinishTime,
                        EntryTimeForTreatment = (DateTime)currentAssignment.EntryTimeForTreatment,
                        CallingDistanceFromVolunteer = Tools.DistanceCalculation(doVolunteer.AddressVolunteer, callDetails.CallAddress),
                        Status = (RiskRangeStatus)Tools.GetCallStatus(callDetails)
                    };
                }
            }
            volunteer.TotalCallsHandled = assigments.Count(a => a.FinishCallType == DO.FinishCallType.TakenCareof);
            volunteer.TotalCallsCanceled = assigments.Count(a => a.FinishCallType == DO.FinishCallType.CanceledByVolunteer);
            volunteer.SelectedAndExpiredCalls = assigments.Count(a => a.FinishCallType == DO.FinishCallType.Expired);
            volunteer.CallInProgress = callInProgress;

            return volunteer;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException("Volunteer not found in data layer.", ex);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("An unexpected error occurred while getting Volunteer details.", ex);
        }
    }

    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to be deleted.</param>
    /// <exception cref="BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    /// <exception cref="BlDeletionImpossible">Thrown if the volunteer has active assignments or calls.</exception>
    /// <exception cref="BlGeneralDatabaseException">Thrown if an error occurs during deletion.</exception>
    public void DeleteVolunteer(int volunteerId)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId)
                       ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");

            IEnumerable<DO.Assignment> volunteerAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
            if (volunteerAssignments.Any())
                throw new BlDeletionImpossible($"Volunteer with ID={volunteerId} cannot be deleted as they have treated/ed calls.");

            _dal.Volunteer.Delete(volunteerId);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5

        }
        catch (DO.DalDoesNotExistException)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
        }
        catch (Exception ex)
        {
            throw new BlDeletionImpossible($"An unexpected error occurred while trying to delete volunteer with ID={volunteerId} .", ex);
        }
    }

    /// <summary>
    /// Updates a volunteer's details in the system.
    /// </summary>
    /// <param name="requesterId">The ID of the requester who is trying to update the volunteer details.</param>
    /// <param name="volunteer">The volunteer object containing the updated details.</param>
    /// <exception cref="BlDoesNotExistException">Thrown if the volunteer or requester does not exist.</exception>
    /// <exception cref="BlInvalidOperationException">Thrown if the requester does not have the required permissions to update the volunteer's details.</exception>
    /// <exception cref="BlGeneralDatabaseException">Thrown if an unexpected error occurs during the update operation.</exception>
    public void UpdateVolunteer(int requesterId, BO.Volunteer volunteer)
    {
        try
        {
            var existingVolunteer = _dal.Volunteer.Read(volunteer.VolunteerId)
                                   ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={volunteer.VolunteerId} does not exist");
            var requester = _dal.Volunteer.Read(requesterId)
                        ?? throw new BlDoesNotExistException($"Requester with ID={requesterId} does not exist");

            if (requester.Role != DO.Role.Manager && requesterId != volunteer.VolunteerId)
            {
                throw new BlInvalidOperationException("Only admins or the volunteer themselves can update details");
            }

            if (requester.Role == DO.Role.Manager && existingVolunteer.Role != (DO.Role)volunteer.Role)
            {
                throw new BlGeneralDatabaseException("Only admins can update the role");
            }
            (double lat, double lon) = Tools.GetCoordinatesFromAddress(volunteer!.AddressVolunteer);

            volunteer.VolunteerLatitude = lat;
            volunteer.VolunteerLongitude = lon;

            var doVolunteer = VolunteerManager.MapToDO(volunteer);
            _dal.Volunteer.Update(doVolunteer);
            VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.VolunteerId); 
            VolunteerManager.Observers.NotifyListUpdated(); 
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"Volunteer with ID={volunteer.VolunteerId} does not exist", ex);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        }
    }

    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="volunteer">The volunteer to be added.</param>
    /// <exception cref="BlDoesNotExistException">Thrown if the volunteer already exists.</exception>
    /// <exception cref="BlGeneralDatabaseException">Thrown if an unexpected error occurs while adding the volunteer.</exception>
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            var existingVolunteer = _dal.Volunteer.Read(v => v.VolunteerId == volunteer.VolunteerId);
            if (existingVolunteer != null)
                throw new DO.DalDoesNotExistException($"Volunteer with ID={volunteer.VolunteerId} already exists.");
            VolunteerManager.ValidateVolunteer(volunteer);

            DO.Volunteer doVolunteer = VolunteerManager.MapToDO(volunteer);
            _dal.Volunteer.Create(doVolunteer);
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"Volunteer with ID={volunteer.VolunteerId} already exists", ex);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        }
    }

    /// <summary>
    /// Retrieves a list of all volunteers.
    /// </summary>
    /// <returns>A list of all volunteers.</returns>
    public IEnumerable<VolunteerInList?> GetVolunteers()
    {
        var volunteers = _dal.Volunteer.ReadAll();

        List<BO.VolunteerInList> volunteerList = new List<BO.VolunteerInList>();

        foreach (DO.Volunteer volunteer in volunteers)
        {
            BO.VolunteerInList volunteerInList = VolunteerManager.MapToVolunteerInList(volunteer);
            volunteerList.Add(volunteerInList);
        }

        return volunteerList;
    }
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5

}

