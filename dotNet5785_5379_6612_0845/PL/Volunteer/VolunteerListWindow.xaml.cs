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
        DataContext = this; 

        SortFields = Enum.GetValues(typeof(BL.BO.TypeSortingVolunteers)).Cast<BL.BO.TypeSortingVolunteers>().ToList();
        RefreshVolunteerList();
        s_bl?.Volunteer.AddObserver(RefreshVolunteerList);
    }
    private void RefreshVolunteerList()
    {
        VolunteerList = s_bl.Volunteer.GetVolunteers(
            sortBy: SelectedSortField == TypeSortingVolunteers.All ? null : SelectedSortField,
            isActive: null
        );
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

    //public BL.BO.CallInList? SelectedCall { get; set; }
    //private void VolunteerDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    //{
    //    if (SelectedVolunteer != null)
    //    {
    //        // פתיחת חלון חדש עם ה-Id של המתנדב
    //        new VolunteerWindow(SelectedVolunteer.VolunteerId).Show();
    //    }
    //}
    //private void AddButton_Click(object sender, RoutedEventArgs e)
    //{
    //    // יצירת חלון חדש של VolunteerWindow במצב הוספה
    //    var addWindow = new VolunteerWindow(); // בלי פרמטר - מצב הוספה
    //    addWindow.ShowDialog(); // ShowDialog כדי שהחלון הנוכחי "יחכה"
    //}
    private  void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int volunteerId)
        {
            var volunteer = VolunteerList?.FirstOrDefault(v => v?.VolunteerId == volunteerId);
            if (volunteer == null)
                return;

            // 1. אישור מחיקה מהמשתמש
            var result = MessageBox.Show($"Are you sure you want to delete volunteer '{volunteer.Name}'?",
                                         "Confirm Deletion",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                // 2. קריאה ל-Delete ב-BL
                s_bl.Volunteer.DeleteVolunteer(volunteerId);

                // 3. אם המחיקה הצליחה, רשימת המתנדבים תתעדכן אוטומטית בזכות המנגנון של observers.
                // כאן אין צורך לקרוא ל-RefreshVolunteerList כי ה-Observer יעשה את זה.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete volunteer:\n{ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var addWindow = new VolunteerWindow(); // פותח את חלון ההוספה
        addWindow.ShowDialog(); // משתמש ב-ShowDialog כדי לחכות לסיום החלון

        // לאחר שהחלון נסגר, אפשר לרענן את הרשימה אם צריך (אם אין Observer)
        // RefreshVolunteerList(); // לא נדרש אם אתה משתמש ב־Observer כמו שכבר עשית
    }



}
