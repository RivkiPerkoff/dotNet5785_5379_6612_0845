using BL.BIApi;
using BL.BO;
using BL.Helpers;
using DO;
using System.Collections.Generic;
using System.Linq;


namespace BL.BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public string Login(string username, string password)
    {
        try
        {
            var volunteer = _dal.Volunteer.ReadAll()
                .FirstOrDefault(v => v.Name == username && v.PasswordVolunteer == password)
                ?? throw new BlInvalidOperationException("Username or password is incorrect");

            return (volunteer.Role).ToString();
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("An unexpected error occurred while getting Volunteers.", ex);
        }
    }
    //public List<BO.VolunteerInList> GetVolunteers(bool? isActive, TypeSortingVolunteers? sortBy) { }

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

            // מיון לפי פרמטרים שנבחרו
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

            return volunteerList; // כבר רשימה של BO.VolunteerInList, אין צורך להמיר שוב
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
                        Status = (RiskRangeStatus)Tools.GetCallStatus(currentAssignment.IdOfRunnerCall)
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
            throw new BlGeneralDatabaseException("An unexpected error occurred while geting Volunteer details.", ex);
        }
    }
    public void DeleteVolunteer(int volunteerId)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId)
                       ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");

            IEnumerable<DO.Assignment> volunteerAssignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
            if (volunteerAssignments.Any())
                throw new BlDeletionImpossible($"Volunteer with ID={volunteerId} cannot be deleted as they have treat/ed calls.");

            _dal.Volunteer.Delete(volunteerId);
        }
        catch (DO.DalDoesNotExistException)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
        }
        catch (Exception ex)
        {
            throw new BlDeletionImpossible($"An unexpected error occurred while trying delete volunteer with ID={volunteerId} .", ex);
        }
    }

    public void UpdateVolunteer(int requesterId, BO.Volunteer volunteer)
    {
        try
        {

            var existingVolunteer = _dal.Volunteer.Read(volunteer.VolunteerId) // עדכון ל-VolunteerId
                                   ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={volunteer.VolunteerId} does not exist");
            var requester = _dal.Volunteer.Read(requesterId)
                        ?? throw new BlDoesNotExistException($"Requester with ID={requesterId} does not exist");

            if (requester.Role != DO.Role.Manager && requesterId != volunteer.VolunteerId) // עדכון ל-VolunteerId
            {
                throw new BlInvalidOperationException("Only admins or the volunteer themselves can update details");
            }

            if (requester.Role == DO.Role.Manager && existingVolunteer.Role != (DO.Role)volunteer.Role)
            {
                throw new BlGeneralDatabaseException("Only admins can update the role");
            }

            var doVolunteer = VolunteerManager.MapToDO(volunteer);


            _dal.Volunteer.Update(doVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"Volunteer with ID={volunteer.VolunteerId} does not exist", ex); // עדכון ל-VolunteerId
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        }

    }
    public void AddVolunteer(BO.Volunteer Volunteer)
    {
        try
        {
            var existingVolunteer = _dal.Volunteer.Read(v => v.VolunteerId == Volunteer.VolunteerId);
            if (existingVolunteer != null)
                throw new DO.DalDoesNotExistException($"Volunteer with ID={Volunteer.VolunteerId} already exists.");
            VolunteerManager.ValidateVolunteer(Volunteer);

            DO.Volunteer doVolunteer = VolunteerManager.MapToDO(Volunteer);
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"Volunteer with ID={Volunteer.VolunteerId} already exists", ex);
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        }
    }
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
}
