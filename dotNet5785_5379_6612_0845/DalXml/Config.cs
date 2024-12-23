namespace Dal;

/// <summary>
/// Static class for managing application configuration and shared data.
/// Contains constants and methods for accessing and updating configuration values in XML files.
/// </summary>
internal static class Config
{
    /// <summary>
    /// Path to the main data configuration XML file.
    /// </summary>
    internal const string s_data_config_xml = "data-config.xml";

    /// <summary>
    /// Path to the volunteers data XML file.
    /// </summary>
    internal const string s_volunteers_xml = "volunteers.xml";

    /// <summary>
    /// Path to the calls data XML file.
    /// </summary>
    internal const string s_calls_xml = "calls.xml";

    /// <summary>
    /// Path to the assignments data XML file.
    /// </summary>
    internal const string s_assignments_xml = "assignments.xml";

    /// <summary>
    /// Gets the next unique ID for a volunteer and increments the value in the configuration file.
    /// </summary>
    internal static int NextVolunteerId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextVolunteerId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextVolunteerId", value);
    }

    /// <summary>
    /// Gets the next unique ID for a call and increments the value in the configuration file.
    /// </summary>
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    /// <summary>
    /// Gets the next unique ID for an assignment and increments the value in the configuration file.
    /// </summary>
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }

    /// <summary>
    /// Gets or sets the current clock value from the configuration file.
    /// </summary>
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    /// <summary>
    /// Resets configuration values to their defaults.
    /// - Sets the next IDs for volunteers, calls, and assignments to 1000.
    /// - Sets the clock to the current time.
    /// </summary>
    internal static void Reset()
    {
        NextVolunteerId = 1000;
        NextCallId = 1000;
        NextAssignmentId = 1000;

        Clock = DateTime.Now;
    }

    /// <summary>
    /// Gets or sets the risk range as a <see cref="TimeSpan"/> value.
    /// The value is stored as a date in the configuration file and only the time part is used.
    /// </summary>
    internal static TimeSpan RiskRange
    {
        get
        {
            DateTime dateTime = XMLTools.GetConfigDateVal(s_data_config_xml, "RiskRange");
            return dateTime.TimeOfDay;
        }
        set
        {
            DateTime dateTime = DateTime.Today.Add(value);
            XMLTools.SetConfigDateVal(s_data_config_xml, "RiskRange", dateTime);
        }
    }
}
