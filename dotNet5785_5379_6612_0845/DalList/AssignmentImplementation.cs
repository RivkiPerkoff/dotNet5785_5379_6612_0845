namespace Dal;
using DalApi;
using DO;
/// <summary>
/// Implementation of the IAssignment interface for managing assignment operations such as create, read, update, delete.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new assignment with a unique volunteer ID.
    /// </summary>
    /// <param name="item">The assignment object to create.</param>
    public void Create(Assignment item)
    {
        int newId = Config.NextVolunteerId;  // Generate a new volunteer ID.
        Assignment newAssignment = item with { VolunteerId = newId };  // Create a new assignment with the generated ID.
        DataSource.Assignments.Add(item);  // Add the assignment to the data source.
    }

    /// <summary>
    /// Deletes an assignment by its volunteer ID.
    /// </summary>
    /// <param name="id">The volunteer ID associated with the assignment to delete.</param>
    /// <exception cref="Exception">Thrown when no assignment with the specified ID is found.</exception>
    public void Delete(int id)
    {
        Assignment? assignment = Read(id);  // Find the assignment by ID.
        if (assignment != null)
        {
            DataSource.Assignments.Remove(assignment);  // Remove the assignment from the data source.
        }
        else
        {
            throw new Exception($"Assignment with Id {id} was not found");  // Throw an exception if no assignment is found.
        }
    }

    /// <summary>
    /// Deletes all assignments in the data source.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();  // Clear all assignments from the data source.
    }

    /// <summary>
    /// Reads an assignment by its volunteer ID.
    /// </summary>
    /// <param name="id">The volunteer ID associated with the assignment to read.</param>
    /// <returns>The assignment with the specified ID, or null if not found.</returns>
    public Assignment? Read(int id)
    {
        Assignment? assignment = DataSource.Assignments.FirstOrDefault(assignment => assignment.VolunteerId == id);  // Find the assignment by volunteer ID.
        return assignment;
    }

    /// <summary>
    /// Reads all assignments from the data source.
    /// </summary>
    /// <returns>A list of all assignments.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) // stage 2
        => filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);


    /// <summary>
    /// Updates an existing assignment by replacing it with the provided item.
    /// </summary>
    /// <param name="item">The assignment object to update.</param>
    /// <exception cref="Exception">Thrown when the assignment with the specified ID is not found.</exception>
    public void Update(Assignment item)
    {
        Assignment? existingAssignment = Read(item.VolunteerId);  // Find the existing assignment.
        if (existingAssignment != null)
        {
            DataSource.Assignments.Remove(existingAssignment);  // Remove the existing assignment from the data source.
            DataSource.Assignments.Add(item);  // Add the updated assignment to the data source.
        }
        else
        {
            throw new Exception($"Could not update item, no assignment with Id {item.VolunteerId} found");  // Throw an exception if assignment is not found.
        }
    }
}
