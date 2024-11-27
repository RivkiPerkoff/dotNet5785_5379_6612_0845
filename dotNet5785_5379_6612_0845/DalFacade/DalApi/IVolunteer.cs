namespace DalApi;
using DO;
/// <summary>
/// Interface for managing volunteer records in the data access layer.
/// Provides methods to create, read, update, and delete volunteer data.
/// </summary>
public interface IVolunteer
{
    public void Create(Volunteer item);
    public void Delete(int id);
    public void DeleteAll();
    public Volunteer? Read(int id);
    public List<Volunteer> ReadAll();
    public void Update(Volunteer item);

}
