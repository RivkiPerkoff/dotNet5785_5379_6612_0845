using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace Dal;

/// <summary>
/// The <c>DalXml</c> class implements the <c>IDal</c> interface and provides access to the XML-based data access layers.
/// It includes modules for managing volunteers, assignments, calls, and configuration settings.
/// </summary>
sealed internal class DalXml : IDal
{
    // public static IDal Instance { get; } = new DalList();
    private static class DalListHolder
    {
        // המשתנה הסטטי נוצר Lazy בצורה Thread Safe
        internal static readonly IDal instance = new DalList();
    }
    // פרסום ה-Instance
    public static IDal Instance => DalListHolder.instance;

    private DalXml() { }
    /// <summary>
    /// Gets the implementation for managing volunteers.
    /// </summary>
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IConfig Config { get; } = new ConfigImplementation();
    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }
}
