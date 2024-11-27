﻿namespace DalApi;
using DO;
/// <summary>
/// Interface for managing assignments in the data access layer.
/// Provides methods to create, read, update, and delete assignment records.
/// </summary>
public interface IAssignment
{
    public void Create(Assignment item);

    public Assignment? Read(int id);

    public List<Assignment> ReadAll();
    public void Update(Assignment item);

    public void Delete(int id);

    public void DeleteAll();
}

