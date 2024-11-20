
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation/* : IVolunteer*/
{
    public void Create(Volunteer item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(int id)
    {
        throw new NotImplementedException();
    }

    public List<Volunteer> ReadAll()
    {
        throw new NotImplementedException();
    }

    //    public void Update(Volunteer item)
    //    {
    //        // חפש אם קיים אובייקט עם ה-ID במאגר הנתונים
    //        var existingItem = _repository.GetVolunteerById(item.VolunteerId);

    //        if (existingItem == null)
    //        {
    //            throw new InvalidOperationException($"אובייקט מסוג Volunteer עם ID {item.VolunteerId} לא קיים.");
    //        }

    //        // אם האובייקט קיים, מחק את הישן והוסף את החדש
    //        _repository.RemoveVolunteer(item.VolunteerId);
    //        _repository.AddVolunteer(item);
    //    }


}

