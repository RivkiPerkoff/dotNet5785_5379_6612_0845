namespace Dal;
internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_assignments_xml = "assignments.xml";
    internal static int NextVolunteerId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextVolunteerId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextVolunteerId", value);
    }
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    internal static void Reset()
    {
        NextVolunteerId = 1000;
        NextCallId = 1000;
        NextAssignmentId = 1000;

        Clock = DateTime.Now;
}
    //internal static TimeSpan? RiskRange { get; set; } = new TimeSpan(1, 30, 0);
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
