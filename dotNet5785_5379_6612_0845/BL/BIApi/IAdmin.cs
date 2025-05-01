using BL.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BIApi;

/// <summary>
/// Interface for Admin service logic.
/// Contains methods required for managing system administration tasks.
/// </summary>
public interface IAdmin
{
    /// <summary>
    /// Gets the current system clock value.
    /// </summary>
    /// <returns>The current system clock value as a DateTime object.</returns>
    DateTime GetClock();

    /// <summary>
    /// Advances the system clock by a specified time unit.
    /// </summary>
    /// <param name="timeUnit">The time unit by which to advance the clock (e.g., Minute, Hour, Day).</param>
    void AdvanceClock(BO.TimeUnit timeUnit);

    /// <summary>
    /// Gets the value of the risk time range configuration.
    /// </summary>
    /// <returns>The risk time range as a TimeSpan object.</returns>
    TimeSpan GetRiskTimeRange();

    /// <summary>
    /// Sets the value of the risk time range configuration.
    /// </summary>
    /// <param name="timeRange">The new risk time range to set.</param>
    void SetRiskTimeRange(TimeSpan timeRange);

    /// <summary>
    /// Resets the database by clearing all data and configuration to default values.
    /// </summary>
    void ResetDatabase();

    /// <summary>
    /// Initializes the database by resetting it and adding initial data for all entities.
    /// </summary>
    void InitializeDatabase();
    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    void AddClockObserver(TimeUnit minute);
    #endregion Stage 5

}
