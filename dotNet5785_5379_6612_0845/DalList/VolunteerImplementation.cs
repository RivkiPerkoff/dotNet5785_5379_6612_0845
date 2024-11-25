namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (DataSource.Volunteers.Any(v => v.VolunteerId == item.VolunteerId))
        {
            throw new InvalidOperationException($"Volunteer with ID {item.VolunteerId} already exists.");
        }

        // הוספת המתנדב ישירות לרשימה
        DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
        var volunteer = DataSource.Volunteers.FirstOrDefault(v => v.VolunteerId == id);

        if (volunteer == null)
        {
            throw new KeyNotFoundException($"Volunteer with ID {id} does not exist.");
        }

        // הסרת המתנדב מהרשימה
        DataSource.Volunteers.Remove(volunteer);
    }

    public void DeleteAll()
    {
        // ריקון הרשימה
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int id)
    {
        // חיפוש מתנדב לפי ID
        return DataSource.Volunteers.FirstOrDefault(volunteer => volunteer.VolunteerId == id);
    }

    public List<Volunteer> ReadAll()
    {
        // החזרת עותק של הרשימה
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        // חיפוש האינדקס של המתנדב לפי ID
        int index = DataSource.Volunteers.FindIndex(v => v.VolunteerId == item.VolunteerId);

        if (index == -1)
        {
            throw new InvalidOperationException($"Update failed: Volunteer with ID {item.VolunteerId} does not exist.");
        }

        // עדכון המתנדב בעמדה המתאימה
        DataSource.Volunteers[index] = item;
    }
}

