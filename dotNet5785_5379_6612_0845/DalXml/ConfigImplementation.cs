namespace Dal;
using DalApi;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementation of the IConfig interface. Provides methods to access configuration values
/// for system clock, risk range, and unique ID generation for volunteers, calls, and assignments.
/// </summary>
internal class ConfigImplementation : IConfig
{
    /// <summary>
    /// Gets or sets the system clock.
    /// </summary>

    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    /// <summary>
    /// Gets or sets the risk range for the system.
    /// </summary>

    public TimeSpan? RiskRange
    {
        get => Config.RiskRange;
        set
        {
            if (value.HasValue)
            {
                Config.RiskRange = value.Value;
            }
            else
            {
                throw new ArgumentNullException(nameof(value), "RiskRange cannot be null.");
            }
        }
    }


    /// <summary>
    /// Creates and returns the next unique volunteer ID.
    /// </summary>
    /// <returns>The next volunteer ID.</returns>
    public int CreateVolunteerId()
    {
        return Config.NextVolunteerId;
    }

    /// <summary>
    /// Creates and returns the next unique call ID.
    /// </summary>
    /// <returns>The next call ID.</returns>
    public int CreateCallId()
    {
       // get => Config.NextCallId;
        return Config.NextCallId;
    }

    /// <summary>
    /// Creates and returns the next unique assignment ID.
    /// </summary>
    /// <returns>The next assignment ID.</returns>
    public int CreateAssignmentId()
    {
        return Config.NextAssignmentId;
    }

    /// <summary>
    /// Resets the system's configuration, including resetting the clock and IDs for volunteers, calls, and assignments.
    /// </summary>
    public void Reset()
    {
        Config.Reset();
    }
}
