using BL.BIApi;
using BL.BO;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerListWindow.xaml
/// </summary>
public partial class VolunteerListWindow : Window
{
    private static readonly IBL s_bl = BlApi.Factory.Get();


    public BL.BO.VolunteerInList? SelectedVolunteer { get; set; }
    public List<BL.BO.TypeSortingVolunteers> SortFields { get; set; }
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
        get { return (IEnumerable<VolunteerInList?>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<VolunteerInList?>), typeof(VolunteerListWindow));
    public VolunteerListWindow()
    {
        InitializeComponent();
        DataContext = this; 


        SortFields = Enum.GetValues(typeof(BL.BO.TypeSortingVolunteers)).Cast<BL.BO.TypeSortingVolunteers>().ToList();
        RefreshVolunteerList();
        s_bl?.Volunteer.AddObserver(RefreshVolunteerList);
    }

    private HashSet<int> observedVolunteerIds = new();
    private void RefreshVolunteerList()
    {
        VolunteerList = s_bl.Volunteer.GetVolunteers(
            sortBy: SelectedSortField == TypeSortingVolunteers.All ? null : SelectedSortField,
            isActive: SelectedIsActive
        );

        foreach (var volunteer in VolunteerList.Where(v => v != null))
        {
            if (observedVolunteerIds.Add(volunteer!.VolunteerId)) 
            {
                s_bl.Volunteer.AddObserver(volunteer.VolunteerId, RefreshVolunteerList);
            }
        }
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

    private BL.BO.TypeSortingVolunteers _selectedSortField = BL.BO.TypeSortingVolunteers.All;

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
    private  void DeleteButton_Click(object sender, RoutedEventArgs e)
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
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Failed to delete volunteer:\n{ex.Message}",
            //                    "Error",
            //                    MessageBoxButton.OK,
            //                    MessageBoxImage.Error);
            //}
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
        var addWindow = new VolunteerWindow(); 
        addWindow.ShowDialog(); 
    }

    private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (SelectedVolunteer != null)
        {
            var win = new VolunteerWindow(SelectedVolunteer.VolunteerId);
            win.ShowDialog();
        }
    }



}
