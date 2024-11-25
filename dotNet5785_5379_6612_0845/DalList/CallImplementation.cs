 
namespace Dal;
using DalApi;
using DO;
using NSubstitute.Core;
using System;
using System.Collections.Generic;
public class CallImplementation : ICall
{
    public void Delete(int id)
    {
        int call = Read(id);
        if (call == null)
            throw new Exception($"Call with Id{id} was found");
        else
            DataSource.Calls.Remove(call);
    }

    public void DeleteAll()

    { DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    { 
        return DataSource.Calls.Find(a => a.IdCall == id);
    }
    public List<Call> ReadAll()

    {
        return DataSource.Calls.FirstOrDefault();
        return DataSource.Volunteers.FirstOrDefault(volunteer => volunteer.VolunteerId == id);

    }
    public void Update(Call item)
    {
        int existingCall = Read(item.Id);
        if (existingCall != null)
        {
            DataSource.Calls.Remove(existingCall);
            DataSource.Calls.Add(item);
        }
        else
        {
            throw new Exception($"Could not Update Item, no Call with Id{item.Id} found");
        }
    }
}
