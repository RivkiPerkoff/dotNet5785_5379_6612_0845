
namespace Dal;
using DalApi;
using DO;
//using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int newId = Config.NextVolunteerId;
        Assignment newAssignment = item with { VolunteerId = newId };
        DataSource.Assignments.Add(item);
    }

    public void Delete(int id)
    {
        Assignment? assignment = Read(id);
        if (assignment != null)
        {
            DataSource.Assignments.Remove(assignment);
        }
        else
        {
            throw new Exception($"Assignment with Id{id} was found");
        }
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        Assignment? assignment = DataSource.Assignments.Find(assignment => assignment.VolunteerId == id);
        return assignment;
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        Assignment? existingAssignment = Read(item.VolunteerId);
        if (existingAssignment != null)
        {
            DataSource.Assignments.Remove(existingAssignment);
            DataSource.Assignments.Add(item);
        }
        else
        {
            throw new Exception($"Could not Update Item, no assignment with Id{item.VolunteerId} found");

        }

    }
}