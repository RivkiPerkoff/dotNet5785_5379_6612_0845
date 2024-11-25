namespace Dal;
using DalApi;
using DO;
using NSubstitute.Core;
using System;
using System.Collections.Generic;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        var newAssignment= new Assignment(item.NextAssignmentId, item.VolunteerId, item.IdOfRunnerCalld, DateTime.Now) 
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Assignment? Read(int id)
    {
        throw new NotImplementedException();
    }

    public List<Assignment> ReadAll()
    {
        throw new NotImplementedException();
    }

    public void Update(Assignment item)
    {
        throw new NotImplementedException();
    }
}

