namespace Dal;
using DalApi;
using System;
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.Clock;

        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.Clock = value;
    }

    /// <summary>
    /// Gets or sets the risk range for the system.
    /// </summary>
    public TimeSpan? RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.RiskRange;

        [MethodImpl(MethodImplOptions.Synchronized)]
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
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int CreateVolunteerId()
    {
        return Config.NextVolunteerId;
    }

    /// <summary>
    /// Creates and returns the next unique call ID.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int CreateCallId()
    {
        return Config.NextCallId;
    }

    /// <summary>
    /// Creates and returns the next unique assignment ID.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int CreateAssignmentId()
    {
        return Config.NextAssignmentId;
    }

    /// <summary>
    /// Resets the system's configuration.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Reset()
    {
        Config.Reset();
    }
}
