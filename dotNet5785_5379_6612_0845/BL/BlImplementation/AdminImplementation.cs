using BL.BIApi;
using BL.BO;
using BL.Helpers;

namespace BL.BlImplementation;
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AdvanceClock(BO.TimeUnit timeUnit)
    {
        DateTime currentClock = ClockManager.Now;

        // יצירת הזמן החדש על פי יחידת הזמן
        DateTime newClock = timeUnit switch
        {
            BO.TimeUnit.Minute => currentClock.AddMinutes(1),
            BO.TimeUnit.Hour => currentClock.AddHours(1),
            BO.TimeUnit.Day => currentClock.AddDays(1),
            BO.TimeUnit.Month => currentClock.AddMonths(1),
            BO.TimeUnit.Year => currentClock.AddYears(1),
            _ => throw new BlDoesNotExistException($"Invalid time unit {nameof(timeUnit)}")
        };

        ClockManager.UpdateClock(newClock);
    }

    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public TimeSpan GetRiskTimeRange()
    {
        // שימוש במשתנה _dal במקום Factory.Get
        return _dal.Config.RiskRange
               ?? throw new BlInvalidOperationException("RiskRange is not set in the configuration.");
    }

    public void SetRiskTimeRange(TimeSpan timeRange)
    {
        // שימוש במשתנה _dal במקום Factory.Get
        _dal.Config.RiskRange = timeRange;
    }

    public void InitializeDatabase()
    {
        DalTest.Initialization.DO();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void ResetDatabase()
    {
        // שימוש במשתנה _dal במקום Factory.Get
        _dal.ResetDB();
        DalTest.Initialization.DO();
        ClockManager.UpdateClock(ClockManager.Now);
    }
}
