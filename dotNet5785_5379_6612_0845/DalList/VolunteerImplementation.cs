
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
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
        throw new NotImplementedException();
    }

    public void Update(Volunteer item)
    {
        throw new NotImplementedException();
    }
}

