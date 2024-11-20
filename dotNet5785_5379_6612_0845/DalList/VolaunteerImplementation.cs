
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolaunteerImplementation : IVolunteer

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

{
    items.Clear();
   }