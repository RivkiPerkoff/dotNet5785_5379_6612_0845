namespace DalApi;
using DO;

public interface IAssignment
{
    public void Create(Assignment item);

    public Assignment? Read(int id);

    public List<Assignment> ReadAll();
    public void Update(Assignment item);

    public void Delete(int id);

    public void DeleteAll();
}

