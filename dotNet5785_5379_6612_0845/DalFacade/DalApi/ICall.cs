namespace DalApi;
using DO;
/// <summary>
/// Interface for managing call records in the data access layer.
/// Provides methods to create, read, update, and delete call records.
/// </summary>
public interface ICall
{
    void Create(Call item);

    Call? Read(int id);

    List<Call> ReadAll();
    void Update(Call item);

    void Delete(int id);

    void DeleteAll();

}
