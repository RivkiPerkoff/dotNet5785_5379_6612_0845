

namespace DalApi;
using DO;
public interface ICall
{
    public void Create(Call item);

    public Call? Read(int id);

    public List<Call> ReadAll();
    public void Update(Call item);

    public void Delete(int id);

    public void DeleteAll();

}
