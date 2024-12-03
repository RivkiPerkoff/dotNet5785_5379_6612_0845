namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
/// <summary>
/// Implementation of the IVolunteer interface, providing functionality for CRUD operations (Create, Read, Update, Delete) on Volunteer entities.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Creates a new volunteer in the system. 
    /// If the volunteer with the same ID already exists, an exception is thrown.
    /// </summary>
    /// <param name="item">The volunteer entity to be created.</param>
    public void Create(Volunteer item)
    {
        if (DataSource.Volunteers.Any(v => v.VolunteerId == item.VolunteerId))
        {
            throw new DalExistException($"Volunteer with ID {item.VolunteerId} already exists.");
        }
        DataSource.Volunteers.Add(item);
    }

    /// <summary>
    /// Deletes a volunteer from the system based on their ID. 
    /// If the volunteer is not found, a KeyNotFoundException is thrown.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be deleted.</param>
    public void Delete(int id)
    {
        var volunteer = DataSource.Volunteers.FirstOrDefault(v => v.VolunteerId == id);

        if (volunteer == null)
        {
            throw new DalDeletionImpossible($"Volunteer with ID {id} does not exist.");
        }

        // Removing the volunteer from the list
        DataSource.Volunteers.Remove(volunteer);
    }

    /// <summary>
    /// Deletes all volunteers from the system, clearing the volunteer list.
    /// </summary>
    public void DeleteAll()
    {
        if (!DataSource.Volunteers.Any())
        {
            throw new DalDeletionImpossible("The Volunteers list is already empty.");
        }
        // Clearing the volunteer list
        DataSource.Volunteers.Clear();
    }

    /// <summary>
    /// Reads a volunteer from the system based on their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be read.</param>
    /// <returns>The volunteer entity, or null if not found.</returns>
    public Volunteer? Read(int id)
    {
        var volunteer = DataSource.Volunteers.FirstOrDefault(volunteer => volunteer.VolunteerId == id);

        // זריקת חריגה אם לא נמצא מתנדב
        if (volunteer == null)
        {
            throw new DalDoesNotExistException($"No Volunteer found with ID {id}.");
        }

        return volunteer;
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        var volunteer = DataSource.Volunteers.FirstOrDefault(filter);

        // זריקת חריגה אם לא נמצא מתנדב מתאים
        if (volunteer == null)
        {
            throw new DalDoesNotExistException("No Volunteer found matching the specified criteria.");
        }

        return volunteer;
    }

    /// <summary>
    /// Retrieves all volunteers from the system.
    /// </summary>
    /// <returns>A list of all volunteers in the system.</returns>
    //public List<Volunteer> ReadAll()
    //{
    //    // Returning a copy of the list
    //    return new List<Volunteer>(DataSource.Volunteers);
    //}
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) // stage 2
    {
        // חיפוש נתונים עם או בלי סינון
        var result = filter == null
            ? DataSource.Volunteers
            : DataSource.Volunteers.Where(filter);

        // זריקת חריגה אם אין נתונים
        if (!result.Any())
        {
            throw new DalReedAllImpossible("No Volunteers found.");
        }

        return result;
    }

    /// <summary>
    /// Updates an existing volunteer in the system. 
    /// If the volunteer is not found, an exception is thrown.
    /// </summary>
    /// <param name="item">The updated volunteer entity.</param>
    public void Update(Volunteer item)
    {
        // Searching for the index of the volunteer by ID
        int index = DataSource.Volunteers.FindIndex(v => v.VolunteerId == item.VolunteerId);

        if (index == -1)
        {
            throw new DalDoesNotExistException($"Update failed: Volunteer with ID {item.VolunteerId} does not exist.");
        }

        // Updating the volunteer at the correct position
        DataSource.Volunteers[index] = item;
    }
}
