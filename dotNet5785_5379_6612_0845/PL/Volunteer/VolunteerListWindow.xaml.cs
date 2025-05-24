using BL.BIApi;
using BL.BO;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerListWindow.xaml
/// </summary>
public partial class VolunteerListWindow : Window
{
    private static readonly IBL s_bl = BlApi.Factory.Get();

    // תכונה רגילה לסינון – לא DependencyProperty
    public BL.BO.VolunteerInList? SelectedVolunteer { get; set; }
    public List<BL.BO.TypeSortingVolunteers> SortFields { get; set; }
    public IEnumerable<VolunteerInList?> VolunteerList
    {
        get { return (IEnumerable<VolunteerInList?>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<VolunteerInList?>), typeof(VolunteerListWindow));


    public VolunteerListWindow()
    {
        InitializeComponent();
        SortFields = Enum.GetValues(typeof(BL.BO.TypeSortingVolunteers)).Cast<BL.BO.TypeSortingVolunteers>().ToList();
        RefreshVolunteerList();
        s_bl?.Volunteer.AddObserver(RefreshVolunteerList);
    }

    private void RefreshVolunteerList()
    {
        VolunteerList = s_bl.Volunteer.GetVolunteers();

        //VolunteerList = (IEnumerable<BL.BO.Volunteer>)s_bl.Volunteer.GetVolunteers(false, SelectedSortField == VolunteerFields.All ? null : BL.Helpers.VolunteerManager.ConvertToTypeSorting(SelectedSortField));
        //isActive: null,
        //sortBy: SelectedSortField == BL.BO.VolunteerFields.All ? null : SelectedSortField
        //filterField: SelectedCallType == BL.BO.CallTypes.None ? null : SelectedCallType);
    }


    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(VolunteerListObserver);
        RefreshVolunteerList();
    }
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
    }

    private BL.BO.TypeSortingVolunteers _selectedSortField = BL.BO.TypeSortingVolunteers.ALL;

    public BL.BO.TypeSortingVolunteers SelectedSortField
    {
        get => _selectedSortField;
        set
        {
            if (_selectedSortField != value)
            {
                _selectedSortField = value;
                RefreshVolunteerList();
            }
        }
    }
    public IEnumerable<BL.BO.CallTypes> CallTypeList { get; } = Enum.GetValues(typeof(BL.BO.CallTypes)).Cast<BL.BO.CallTypes>().ToList();

    private BL.BO.CallTypes? _selectedCallType = null;
    public BL.BO.CallTypes? SelectedCallType
    {
        get => _selectedCallType;
        set
        {
            if (_selectedCallType != value)
            {
                _selectedCallType = value;
                RefreshVolunteerList();
            }
        }
    }
    private void VolunteerListObserver()
=> RefreshVolunteerList();

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        // לחצן "Add" – ניתן להוסיף כאן פתיחה של חלון חדש להוספת מתנדב
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // שינוי מתנדב נבחר – אפשר להשתמש בזה להצגת פרטים
    }
    //public BL.BO.VolunteerFields SelectedVolunteerField { get; set; } = BL.BO.VolunteerFields;
    //// C#
    //private void VolunteerFieldComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    // ערך SelectedVolunteerField כבר התעדכן אוטומטית
    //    // עכשיו עדכן את הרשימה לפי הערך החדש
    //    CallList = s_bl.Call.GetFilteredAndSortedCallList(
    //        filterBy: (BO.CallInListFields?)SelectedVolunteerField,
    //    );
    //}
}
