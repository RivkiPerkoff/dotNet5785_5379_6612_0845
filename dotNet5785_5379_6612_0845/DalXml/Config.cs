namespace Dal;
using System;
using System.Runtime.CompilerServices;

internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_assignments_xml = "assignments.xml";

    internal static int NextVolunteerId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextVolunteerId");

        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextVolunteerId", value);
    }

    internal static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");

        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    internal static int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");

        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }

    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");

        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        NextVolunteerId = 1000;
        NextCallId = 1000;
        NextAssignmentId = 1000;
        Clock = DateTime.Now;
    }

    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
            DateTime dateTime = XMLTools.GetConfigDateVal(s_data_config_xml, "RiskRange");
            return dateTime.TimeOfDay;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            DateTime dateTime = DateTime.Today.Add(value);
            XMLTools.SetConfigDateVal(s_data_config_xml, "RiskRange", dateTime);
        }
    }
}
