using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BL.BIApi;
using BL.BO;

namespace PL.Volunteer;

public partial class VolunteerWindow : Window, INotifyPropertyChanged
{
    private static readonly IBL s_bl = BlApi.Factory.Get();

    private BL.BO.Volunteer _currentVolunteer;
    public BL.BO.Volunteer CurrentVolunteer
    {
        get => _currentVolunteer;
        set
        {
            _currentVolunteer = value;
            OnPropertyChanged(nameof(CurrentVolunteer));
        }
    }

    private string _buttonText;
    public string ButtonText
    {
        get => _buttonText;
        set
        {
            _buttonText = value;
            OnPropertyChanged(nameof(ButtonText));
        }
    }

    public VolunteerWindow(int id = 0)
    {
        InitializeComponent();
        DataContext = this;

        CurrentVolunteer = id != 0 ? s_bl.Volunteer.GetVolunteerDetails(id) : new BL.BO.Volunteer();

        if (CurrentVolunteer?.VolunteerId != 0)
        {
            s_bl.Volunteer.AddObserver(CurrentVolunteer.VolunteerId, RefreshCurrentVolunteer);
        }

        ButtonText = id == 0 ? "Add" : "Update";

        this.Loaded += VolunteerWindow_Loaded;
        this.Closing += VolunteerWindow_Closing;
    }

    private void VolunteerWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.VolunteerId != 0)
        {
            s_bl.Volunteer.AddObserver(CurrentVolunteer.VolunteerId, RefreshCurrentVolunteer);
        }
    }

    private void VolunteerWindow_Closing(object sender, CancelEventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.VolunteerId != 0)
        {
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer.VolunteerId, RefreshCurrentVolunteer);
        }
    }

    private void RefreshCurrentVolunteer()
    {
        int id = CurrentVolunteer.VolunteerId;
        CurrentVolunteer = null;
        CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
    }

    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {

        try
        {
            if (ButtonText == "Add")
            {
                s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                MessageBox.Show("התנדבות נוספה בהצלחה!", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (ButtonText == "Update")
            {
                s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.VolunteerId, CurrentVolunteer);
                MessageBox.Show("התנדבות עודכנה בהצלחה!", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("מצב כפתור לא ידוע.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.Close();
        }
        catch (BL.BO.BlDoesNotExistException ex)
        {
            MessageBox.Show(ex.Message, "שגיאה ב־BL", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show("אירעה שגיאה לא צפויה: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            CurrentVolunteer.PasswordVolunteer = passwordBox.Password;
        }
    }
    public class VolunteerIdToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id && id > 0)
                return "עדכן מתנדב";
            return "הוסף מתנדב";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
