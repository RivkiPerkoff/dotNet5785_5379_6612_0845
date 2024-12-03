namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Implementation of the ICall interface for managing call operations such as create, read, update, delete.
/// </summary>
internal class CallImplementation : ICall
{
    /// <summary>
    /// Creates a new call and adds it to the data source.
    /// </summary>
    /// <param name="item">The call object to create.</param>
    /// <exception cref="Exception">Thrown when a call with the same ID already exists.</exception>
    public void Create(Call item)
    {
        if (DataSource.Calls.Exists(a => a.IdCall == item.IdCall))
        {
            throw new DalExistException($"Call with Id {item.IdCall} already exists");  // Throw an exception if the call already exists.
        }
        DataSource.Calls.Add(item);  // Add the new call to the data source.
    }

    /// <summary>
    /// Deletes a call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call to delete.</param>
    /// <exception cref="Exception">Thrown when no call with the specified ID is found.</exception>
    public void Delete(int id)
    {
        Call? call = Read(id);  // Find the call by ID.
        if (call == null)
        {
            throw new DalDeletionImpossible($"Call with Id {id} was not found");  // Throw an exception if the call is not found.
        }
        else
        {
            DataSource.Calls.Remove(call);  // Remove the call from the data source.
        }
    }

    /// <summary>
    /// Deletes all calls from the data source.
    /// </summary>
    public void DeleteAll()
    {
        if (!DataSource.Calls.Any())
        {
            throw new DalDeletionImpossible("The Calls list is already empty.");
        }
        DataSource.Calls.Clear();  // Clear all calls from the data source.
    }

    /// <summary>
    /// Reads a call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call to read.</param>
    /// <returns>The call with the specified ID, or null if not found.</returns>
    public Call? Read(int id)
    {
        //return DataSource.Calls.FirstOrDefault(call => call.IdCall == id);  // Find the call by ID.
        var call = DataSource.Calls.FirstOrDefault(volunteer => volunteer.IdCall == id);

        // זריקת חריגה אם לא נמצא מתנדב
        if (call == null)
        {
            throw new DalDoesNotExistException($"No Call found with ID {id}.");
        }

        return call;
    }

    public Call? Read(Func<Call, bool> filter)
    {
        // Use LINQ to find the first Call object that matches the filter criteria.
        //return DataSource.Calls.FirstOrDefault(filter);
        var call = DataSource.Calls.FirstOrDefault(filter);

        // זריקת חריגה אם לא נמצא מתנדב מתאים
        if (call == null)
        {
            throw new DalDoesNotExistException("No Call found matching the specified criteria.");
        }

        return call;
    }

    /// <summary>
    /// Reads all calls from the data source.
    /// </summary>
    /// <returns>A list of all calls.</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) // stage 2
    {
        // חיפוש נתונים עם או בלי סינון
        var result = filter == null
            ? DataSource.Calls.Select(item => item)
            : DataSource.Calls.Where(filter);

        // זריקת חריגה אם אין נתונים
        if (!result.Any())
        {
            throw new DalReedAllImpossible("No Calls found.");
        }

        return result;
    }

    /// <summary>
    /// Updates an existing call by replacing it with the provided item.
    /// </summary>
    /// <param name="item">The call object to update.</param>
    /// <exception cref="Exception">Thrown when no call with the specified ID is found.</exception>
    public void Update(Call item)
    {
        var existingCall = Read(item.IdCall);  // Find the existing call by ID.
        if (existingCall != null)
        {
            DataSource.Calls.Remove(existingCall);  // Remove the existing call from the data source.
            DataSource.Calls.Add(item);  // Add the updated call to the data source.
        }
        else
        {
            throw new DalDoesNotExistException($"Could not update item, no Call with Id {item.IdCall} found");  // Throw an exception if no matching call is found.
        }
    }
}
