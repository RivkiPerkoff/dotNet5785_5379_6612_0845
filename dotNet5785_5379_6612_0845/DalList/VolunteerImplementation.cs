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


//namespace Dal;
//using DalApi;
//using DO;
//using System.Collections.Generic;

//internal class VolunteerImplementation: IVolunteer
//{
//    public void Create(Volunteer item)
//    {
//        if (DataSource.Volunteers.Any(v => v.VolunteerId == item.VolunteerId))
//        {
//            throw new InvalidOperationException($"Volunteer with ID {item.VolunteerId} already exists.");
//        }
//        var volunteersList = DataSource.Volunteers.ToList(); // המרת IEnumerable לרשימה
//        volunteersList.Add(item); // הוספת האובייקט
//        DataSource.Volunteers = volunteersList; // עדכון רשימת המתנדבים במקור הנתונים
//    }

//    public void Delete(int id)
//    {
//        // חיפוש האובייקט ברשימה עם ה-ID שהתקבל
//        var volunteer = DataSource.Volunteers.FirstOrDefault(v => v.VolunteerId == id);

//        // אם האובייקט לא נמצא, נזרוק חריגה מתאימה
//        if (volunteer == null)
//        {
//            throw new KeyNotFoundException($"Volunteer with ID {id} does not exist.");
//        }

//        // הסרת האובייקט מהרשימה
//        DataSource.Volunteers.Remove(volunteer);
//    }

//    public void DeleteAll()
//    {
//        while (DataSource.Volunteers.Count > 0)
//        {
//            DataSource.Volunteers.RemoveAt(DataSource.Volunteers.Count - 1);
//        }
//    }

//    public Volunteer? Read(int id)
//    {
//        // חיפוש האובייקט ברשימת המתנדבים לפי ID
//        return DataSource.Volunteers.FirstOrDefault(volunteer => volunteer.VolunteerId == id);
//    }

//        public List<Volunteer> ReadAll()
//    {
//        // יצירת עותק של הרשימה הקיימת של כל האובייקטים מטיפוס Volunteer
//        return new List<Volunteer>(DataSource.Volunteers);
//    }

//    public void Update(Volunteer item)
//    {
//        // המרת IEnumerable לרשימה
//        var volunteerList = DataSource.Volunteers.ToList();

//        // חפש אם קיים אובייקט עם ה-ID במאגר הנתונים
//        var existingItem = volunteerList.FirstOrDefault(v => v.VolunteerId == item.VolunteerId);

//        // אם האובייקט לא קיים, נזרוק חריגה
//        if (existingItem == null)
//        {
//            throw new InvalidOperationException($"update faild: אובייקט מסוג Volunteer עם ID {item.VolunteerId} לא קיים.");
//        }

//        // מצא את האובייקט עם ה-ID ונעדכן אותו במקום להסיר ולהוסיף מחדש
//        int index = volunteerList.FindIndex(v => v.VolunteerId == item.VolunteerId);

//        if (index >= 0)
//        {
//            // עדכון של האובייקט בעמדה הנכונה ברשימה
//            volunteerList[index] = item;

//            // עדכון הרשימה ב-DataSource
//            DataSource.Volunteers = volunteerList;
//        }
//    }
//}

