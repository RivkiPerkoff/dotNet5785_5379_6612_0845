

namespace DalApi;
using DO;
public interface IStudent
{
    void Create(Student item); //Creates new entity object in DAL
    Student? Read(int id); //Reads entity object by its ID
    List<Student> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(Student item); //Updates entity object
    void Delete(int id); //Deletes an object by is Id
    void DeleteAll(); //Delete all entity objects
}