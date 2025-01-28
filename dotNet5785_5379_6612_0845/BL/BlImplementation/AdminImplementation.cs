using BL.BIApi;
using BL.Helpers;

namespace BL.BlImplementation;
internal class AdminImplementation : IAdmin
{
    public DateTime AdvanceClock(BO.TimeUnit timeUnit)
    {
       return ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));
        throw new NotImplementedException();
    }

    public DateTime GetClock()
    {
        return ClockManager.Now;

        throw new NotImplementedException();
    }

    public TimeSpan GetRiskTimeRange()
    {
        throw new NotImplementedException();
    }

    public void InitializeDatabase()
    {
        throw new NotImplementedException();
    }

    public void ResetDatabase()
    {
        throw new NotImplementedException();
    }

    public void SetRiskTimeRange(TimeSpan timeRange)
    {
        throw new NotImplementedException();
    }

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

}
