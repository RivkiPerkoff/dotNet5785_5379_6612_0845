namespace DalApi;
using DO;

public interface IAssignment
{
     void Create(Assignment item);

     Assignment? Read(int id);

     List<Assignment> ReadAll();
     void Update(Assignment item);

     void Delete(int id);

     void DeleteAll();
}

