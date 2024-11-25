namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        if (DataSource.Calls.Exists(a => a.IdCall == item.IdCall))
        {
            throw new Exception($"Call with Id {item.IdCall} already exists");
        }
        DataSource.Calls.Add(item);
    }
    public void Delete(int id)
    {
        var call = Read(id);
        if (call == null)
        {
            throw new Exception($"Call with Id {id} was not found");
        }
        else
        {
            DataSource.Calls.Remove(call);
        }
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        return DataSource.Calls.Find(a => a.IdCall == id); // Assumes 'IdCall' is the correct field.
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        var existingCall = Read(item.IdCall); // Ensure 'IdCall' matches the field name in Call.
        if (existingCall != null)
        {
            DataSource.Calls.Remove(existingCall);
            DataSource.Calls.Add(item);
        }
        else
        {
            throw new Exception($"Could not update item, no Call with Id {item.IdCall} found");
        }
    }
}
