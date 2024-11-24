﻿
namespace DalApi;
using DO;
public interface IVolunteer
{
    public void Create(Volunteer item);
    public void Delete(int id);
    public void DeleteAll();
    public Volunteer? Read(int id);
    public List<Volunteer> ReadAll();
    public void Update(Volunteer item);

}
