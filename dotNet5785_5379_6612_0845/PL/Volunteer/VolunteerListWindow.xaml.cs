using BL.BIApi;
using BL.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL.Volunteer;

public partial class VolunteerListWindow : Window
{
    private static readonly IBL s_bl = BlApi.Factory.Get();

    public VolunteerInList? SelectedVolunteer { get; set; }
    public List<TypeSortingVolunteers> SortFields { get; set; }

    private bool? _selectedIsActive = null;
    public bool? SelectedIsActive
    {
        get => _selectedIsActive;
        set
        {
            if (_selectedIsActive != value)
            {
                _selectedIsActive = value;
                RefreshVolunteerList();
            }
        }
    }

    public IEnumerable<VolunteerInList?> VolunteerList
    {
        get => (IEnumerable<VolunteerInList?>)GetValue(VolunteerListProperty);
        set => SetValue(VolunteerListProperty, value);
    }

    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<VolunteerInList?>), typeof(VolunteerListWindow));

    private readonly HashSet<int> observedVolunteerIds = new();

    private volatile DispatcherOperation? _refreshOperation = null;
    private volatile DispatcherOperation? _observerOperation = null;

    public VolunteerListWindow()
    {
        InitializeComponent();
        DataContext = this;

        SortFields = Enum.GetValues(typeof(TypeSortingVolunteers)).Cast<TypeSortingVolunteers>().ToList();
        RefreshVolunteerList();
        s_bl.Volunteer.AddObserver(RefreshVolunteerList);
    }

    private void RefreshVolunteerList()
    {
        if (_refreshOperation != null && !_refreshOperation.Status.HasFlag(DispatcherOperationStatus.Completed))
            return;

        _refreshOperation = Dispatcher.InvokeAsync(() =>
        {
            VolunteerList = s_bl.Volunteer.GetVolunteers(
                sortBy: SelectedSortField == TypeSortingVolunteers.All ? null : SelectedSortField,
                isActive: SelectedIsActive
            );

            foreach (var volunteer in VolunteerList.Where(v => v != null))
            {
                if (observedVolunteerIds.Add(volunteer!.VolunteerId))
                    s_bl.Volunteer.AddObserver(volunteer.VolunteerId, RefreshVolunteerList);
            }
        });
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

    private TypeSortingVolunteers _selectedSortField = TypeSortingVolunteers.All;
    public TypeSortingVolunteers SelectedSortField
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

    public IEnumerable<CallTypes> CallTypeList { get; } = Enum.GetValues(typeof(CallTypes)).Cast<CallTypes>().ToList();

    private CallTypes? _selectedCallType = null;
    public CallTypes? SelectedCallType
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
    {
        if (_observerOperation != null && !_observerOperation.Status.HasFlag(DispatcherOperationStatus.Completed))
            return;

        _observerOperation = Dispatcher.InvokeAsync(() => RefreshVolunteerList());
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int volunteerId)
        {
            var volunteer = VolunteerList?.FirstOrDefault(v => v?.VolunteerId == volunteerId);
            if (volunteer == null)
                return;

            var result = MessageBox.Show($"Are you sure you want to delete volunteer '{volunteer.Name}'?",
                                         "Confirm Deletion",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                s_bl.Volunteer.DeleteVolunteer(volunteerId);
                MessageBox.Show("Volunteer deleted successfully.");
            }
            catch (Exception ex)
            {
                var fullMessage = $"{ex.Message}";
                if (ex.InnerException != null)
                    fullMessage += $"\nInner: {ex.InnerException.Message}";
                MessageBox.Show(fullMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        //var addWindow = new CallInProgressDisplay();
        //addWindow.ShowDialog();
        var addWindow = new VolunteerWindow();
        addWindow.ShowDialog();

    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedVolunteer != null)
        {
            //var win = new CallInProgressDisplay(SelectedVolunteer.VolunteerId);
            //win.ShowDialog();
            var win = new VolunteerWindow(SelectedVolunteer.VolunteerId);
            win.ShowDialog();

        }
    }
}
