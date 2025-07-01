using BL.BIApi;
using BL.BO;
using BL.Helpers;
using Helpers;

namespace BL.BlImplementation;
/// <summary>
/// Implementation of administrative functionalities, including clock management,
/// risk time configuration, and database operations.
/// </summary>
internal class AdminImplementation : IAdmin
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public event Action ClockUpdatedObservers;

    /// <summary>
    /// Advances the system clock based on the specified time unit.
    /// </summary>
    /// <param name="timeUnit">The unit of time to advance the clock.</param>
    /// <exception cref="BlDoesNotExistException">Thrown when an invalid time unit is provided.</exception>
    public void AdvanceClock(BO.TimeUnit timeUnit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        DateTime currentClock = AdminManager.Now;

        DateTime newClock = timeUnit switch
        {
            BO.TimeUnit.Minute => currentClock.AddMinutes(1),
            BO.TimeUnit.Hour => currentClock.AddHours(1),
            BO.TimeUnit.Day => currentClock.AddDays(1),
            BO.TimeUnit.Month => currentClock.AddMonths(1),
            BO.TimeUnit.Year => currentClock.AddYears(1),
            _ => throw new BlDoesNotExistException($"Invalid time unit {nameof(timeUnit)}")
        };

        AdminManager.UpdateClock(newClock);
        ClockUpdatedObservers?.Invoke();

    }

    /// <summary>
    /// Retrieves the current system clock.
    /// </summary>
    /// <returns>The current date and time.</returns>
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    /// <summary>
    /// Gets the configured risk time range from the database.
    /// </summary>
    /// <returns>The risk time range as a TimeSpan.</returns>
    /// <exception cref="BlInvalidOperationException">Thrown when RiskRange is not set.</exception>
    public TimeSpan GetRiskTimeRange()
    {
        return AdminManager.MaxRange
               ?? throw new BlInvalidOperationException("RiskRange is not set in the configuration.");
    }

    /// <summary>
    /// Sets a new risk time range in the database.
    /// </summary>
    /// <param name="timeRange">The time span to set as the new risk range.</param>
    public void SetRiskTimeRange(TimeSpan timeRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.MaxRange = timeRange;
    }

    /// <summary>
    /// Initializes the database and resets the system clock.
    /// </summary>
    public void InitializeDatabase()
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.InitializeDB(); 
    }

    /// <summary>
    /// Resets the database and updates the system clock.
    /// </summary>
    public void ResetDatabase()
    {
        AdminManager.ThrowOnSimulatorIsRunning(); 
        AdminManager.ResetDB();
    }
    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    /// <summary>
    /// Starts the simulator if it's not already running. 
    /// Throws an exception if the simulator is already active.
    /// </summary>
    /// <param name="interval">The interval in milliseconds between simulator actions.</param>
    public void StartSimulator(int interval)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.Start(interval);
    }

    /// <summary>
    /// Stops the simulator if it is currently running.
    /// </summary>
    public void StopSimulator()
    {
        AdminManager.Stop();
    }
    #endregion Stage 5

}
