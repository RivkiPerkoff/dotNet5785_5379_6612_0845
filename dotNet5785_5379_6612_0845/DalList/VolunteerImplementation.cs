
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation: IVolunteer
{
    public void Create(Volunteer item)
    {
        // אם המספר המזהה של האובייקט הוא ברירת מחדל, נטפל בו כמספר רץ
        if (item.ID == default)
        {
            // יצירת מספר מזהה חדש
            int newId = Config.NextVolunteerId;

            // יצירת העתק של האובייקט ועדכון ה-ID שלו
            Volunteer newItem = new Volunteer
            {
                ID = newId,
                Name = item.Name,
                Age = item.Age,
                // הוסף שדות נוספים מתוך האובייקט לפי הצורך
            };

            // הוספה לרשימת האובייקטים
            DataSource.Volunteers.Add(newItem);

            // אין צורך בערך חוזר לפי ההוראות
            return;
        }

        // אם המספר המזהה כבר נקבע, נוודא שאין כפילות
        if (DataSource.Volunteers.Any(v => v.ID == item.ID))
        {
            throw new InvalidOperationException($"Volunteer with ID {item.ID} already exists.");
        }

        // הוספה לרשימת האובייקטים
        DataSource.Volunteers.Add(item);

        // אין צורך בערך חוזר לפי ההוראות
    }


    public void Delete(int id)
    {
        // חיפוש האובייקט ברשימה עם ה-ID שהתקבל
        var volunteer = DataSource.Volunteers.FirstOrDefault(v => v.ID == id);

        // אם האובייקט לא נמצא, נזרוק חריגה מתאימה
        if (volunteer == null)
        {
            throw new KeyNotFoundException($"Volunteer with ID {id} does not exist.");
        }

        // הסרת האובייקט מהרשימה
        DataSource.Volunteers.Remove(volunteer);
    }


    public void DeleteAll()
    {
        items.Clear();
    }

    public Volunteer? Read(int id)
    {
        return items.FirstOrDefault(item => idSelector(item) == id);
    }


    public List<Volunteer> ReadAll()
    {
        // יצירת עותק של הרשימה הקיימת של כל האובייקטים מטיפוס Volunteer
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        // המרת IEnumerable לרשימה
        var volunteerList = DataSource.Volunteers.ToList();

        // חפש אם קיים אובייקט עם ה-ID במאגר הנתונים
        var existingItem = volunteerList.FirstOrDefault(v => v.VolunteerId == item.VolunteerId);

        // אם האובייקט לא קיים, נזרוק חריגה
        if (existingItem == null)
        {
            throw new InvalidOperationException($"אובייקט מסוג Volunteer עם ID {item.VolunteerId} לא קיים.");
        }

        // מצא את האובייקט עם ה-ID ונעדכן אותו במקום להסיר ולהוסיף מחדש
        int index = volunteerList.FindIndex(v => v.VolunteerId == item.VolunteerId);

        if (index >= 0)
        {
            // עדכון של האובייקט בעמדה הנכונה ברשימה
            volunteerList[index] = item;

            // עדכון הרשימה ב-DataSource
            DataSource.Volunteers = volunteerList;
        }
    }

}

