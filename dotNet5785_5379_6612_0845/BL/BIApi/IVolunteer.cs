

namespace BL.BIApi;

/// <summary>
/// Interface for Volunteer service logic.
/// Contains methods required for managing and interacting with volunteers.
/// </summary>
public interface IVolunteer
{
    /// <summary>
    /// Login method for the system.
    /// Validates the username and password and returns the user's role.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>The role of the user.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the username or password is incorrect.</exception>
    string Login(string username, string password);

    /// <summary>
    /// Retrieves a list of volunteers, optionally filtered by their active status and sorted by a specific field.
    /// </summary>
    /// <param name="isActive">Nullable boolean to filter volunteers by active status. Null returns all.</param>
    /// <param name="sortBy">Nullable enum to specify the sorting field. Null defaults to ID.</param>
    /// <returns>A sorted and filtered list of volunteers.</returns>
    List<BO.VolunteerInList> GetVolunteers(bool? isActive, BO.TypeSortingVolunteers? sortBy);

    /// <summary>
    /// Retrieves the details of a specific volunteer by ID.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>The Volunteer business object containing details.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the volunteer does not exist.</exception>
    BO.Volunteer GetVolunteerDetails(int volunteerId);

    /// <summary>
    /// Updates the details of a specific volunteer.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the update.</param>
    /// <param name="volunteer">The updated volunteer business object.</param>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown if the requester is not an admin or the same volunteer.
    /// </exception>
    /// <exception cref="ArgumentException">Thrown if any of the provided data is invalid.</exception>
    void UpdateVolunteer(int requesterId, BO.Volunteer volunteer);

    /// <summary>
    /// Deletes a volunteer by their ID.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to delete.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the volunteer is currently handling a task or has handled any tasks in the past.
    /// </exception>
    /// <exception cref="KeyNotFoundException">Thrown if the volunteer does not exist.</exception>
    void DeleteVolunteer(int volunteerId);

    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="volunteer">The new volunteer business object to add.</param>
    /// <exception cref="ArgumentException">Thrown if any of the provided data is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown if a volunteer with the same ID already exists.</exception>
    void AddVolunteer(BO.Volunteer volunteer);
}