using BL.BIApi;
using BL.BO;
using BL.Helpers;

namespace BL.BlImplementation;
/// <summary>
/// Implementation of administrative functionalities, including clock management,
/// risk time configuration, and database operations.
/// </summary>
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Advances the system clock based on the specified time unit.
    /// </summary>
    /// <param name="timeUnit">The unit of time to advance the clock.</param>
    /// <exception cref="BlDoesNotExistException">Thrown when an invalid time unit is provided.</exception>
    public void AdvanceClock(BO.TimeUnit timeUnit)
    {
        DateTime currentClock = ClockManager.Now;

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

    /// <summary>
    /// Retrieves the current system clock.
    /// </summary>
    /// <returns>The current date and time.</returns>
    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    /// <summary>
    /// Gets the configured risk time range from the database.
    /// </summary>
    /// <returns>The risk time range as a TimeSpan.</returns>
    /// <exception cref="BlInvalidOperationException">Thrown when RiskRange is not set.</exception>
    public TimeSpan GetRiskTimeRange()
    {
        return _dal.Config.RiskRange
               ?? throw new BlInvalidOperationException("RiskRange is not set in the configuration.");
    }

    /// <summary>
    /// Sets a new risk time range in the database.
    /// </summary>
    /// <param name="timeRange">The time span to set as the new risk range.</param>
    public void SetRiskTimeRange(TimeSpan timeRange)
    {
        _dal.Config.RiskRange = timeRange;
    }

    /// <summary>
    /// Initializes the database and resets the system clock.
    /// </summary>
    public void InitializeDatabase()
    {
        DalTest.Initialization.DO();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    /// <summary>
    /// Resets the database and updates the system clock.
    /// </summary>
    public void ResetDatabase()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }
}
