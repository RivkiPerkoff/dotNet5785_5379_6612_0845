using System.Windows;
using BL.BO;
using BlApi;
using System.Collections.Generic;
using System.Linq;
using System;
using BL.BIApi;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Threading;

namespace PL.Volunteer;

public partial class VolunteerWindow : Window, INotifyPropertyChanged
{
    private static readonly IBL volunteer_bl = BlApi.Factory.Get();
    public string CurrentCallInfo =>
    CurrentVolunteer?.CallInProgress == null
        ? "No active call"
        : $"Address: {CurrentVolunteer.CallInProgress.CallingAddress}\n" +
          $"Status: {CurrentVolunteer.CallInProgress.Status}\n" +
          $"Distance: {CurrentVolunteer.CallInProgress.CallingDistanceFromVolunteer} km";

    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set
        {
            SetValue(ButtonTextProperty, value);
            OnPropertyChanged(nameof(ButtonText));
            OnPropertyChanged(nameof(IsEditMode));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Add"));
    public bool IsEditMode => ButtonText == "Update";

    public IEnumerable<BL.BO.Role> RoleCollection { get; set; }
    public IEnumerable<BL.BO.DistanceType> DistanceTypeCollection { get; set; }

    public BL.BO.Volunteer? CurrentVolunteer
    {
        get => (BL.BO.Volunteer?)GetValue(CurrentVolunteerProperty);
        set => SetValue(CurrentVolunteerProperty, value);
    }

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register(
            nameof(CurrentVolunteer),
            typeof(BL.BO.Volunteer),
            typeof(VolunteerWindow),
            new PropertyMetadata(null));

    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register(
            nameof(Password),
            typeof(string),
            typeof(VolunteerWindow),
            new PropertyMetadata(string.Empty));

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.VolunteerId != 0)
            volunteer_bl.Volunteer.AddObserver(CurrentVolunteer.VolunteerId, RefreshVolunteer);
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.VolunteerId != 0)
            volunteer_bl.Volunteer.RemoveObserver(CurrentVolunteer.VolunteerId, RefreshVolunteer);
    }
    public VolunteerWindow(int id = 0)
    {
        InitializeComponent();
        Loaded += Window_Loaded;
        Closed += Window_Closed;
        ButtonText = id != 0 ? "Update" : "Add";



        RoleCollection = Enum.GetValues(typeof(BL.BO.Role)).Cast<BL.BO.Role>();
        DistanceTypeCollection = Enum.GetValues(typeof(BL.BO.DistanceType)).Cast<BL.BO.DistanceType>();

        if (id != 0)
        {

            var volunteer = volunteer_bl.Volunteer.GetVolunteerDetails(id);
            if (volunteer != null)
            {
                CurrentVolunteer = volunteer;
            }
            else
            {
                MessageBox.Show("Volunteer not found.");
                Close();
            }
        }
        else
        {
            CurrentVolunteer = new BL.BO.Volunteer
            {
                VolunteerId = 0,
                Name = "",
                PhoneNumber = "",
                EmailOfVolunteer = "",
                AddressVolunteer = "",
                IsAvailable = false,
                VolunteerLatitude = 0,
                VolunteerLongitude = 0,
                MaximumDistanceForReceivingCall = 0,
                DistanceType = BL.BO.DistanceType.AirDistance,
                Role = BL.BO.Role.Volunteer
            };
        }

        DataContext = this;
    }

    //private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        if (CurrentVolunteer == null)
    //            return;

    //        CurrentVolunteer.PasswordVolunteer = Password;


    //        if (ButtonText == "Add")
    //        {
    //            volunteer_bl.Volunteer.AddVolunteer(CurrentVolunteer);
    //            MessageBox.Show("Volunteer added successfully.");
    //        }
    //        else
    //        {
    //            volunteer_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.VolunteerId, CurrentVolunteer);
    //            MessageBox.Show("Volunteer updated successfully.");
    //        }

    //        Password = "";
    //        lastPasswordBox?.Clear();
    //        Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show($"Error: {ex?.InnerException?.Message}");
    //    }
    //}
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentVolunteer == null)
                return;

            // אם זה עדכון ולא הוזנה סיסמה – לא לאפשר עדכון
            if (ButtonText == "Update" && string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Password must be entered to update volunteer details.", "Missing Password", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CurrentVolunteer.PasswordVolunteer = Password;

            bool isSelfDemotion =
                ButtonText == "Update" &&
                volunteer_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.VolunteerId).Role == BL.BO.Role.Manager &&
                CurrentVolunteer.Role == BL.BO.Role.Volunteer;

            if (ButtonText == "Add")
            {
                volunteer_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                MessageBox.Show("Volunteer added successfully.");
            }
            else
            {
                volunteer_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.VolunteerId, CurrentVolunteer);
                MessageBox.Show("Volunteer updated successfully.");
            }

            Password = "";
            lastPasswordBox?.Clear();

            if (isSelfDemotion)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();

                foreach (Window w in Application.Current.Windows.OfType<Window>().ToList())
                {
                    if (w != loginWindow)
                        w.Close();
                }

                return;
            }

            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex?.InnerException?.Message ?? ex.Message}");
        }
    }



    private PasswordBox? lastPasswordBox;

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = sender as PasswordBox;
        if (passwordBox != null)
        {
            Password = passwordBox.Password;
            lastPasswordBox = passwordBox;
        }
    }

    // שדה חדש למניעת ריבוי קריאות למתודת ההשקפה
    private volatile DispatcherOperation? _refreshOperation = null;

    // מתודת השקפה מעודכנת
    private void RefreshVolunteer()
    {
        if (_refreshOperation != null && !_refreshOperation.Status.HasFlag(DispatcherOperationStatus.Completed))
            return;

        _refreshOperation = Dispatcher.InvokeAsync(() =>
        {
            if (CurrentVolunteer == null)
                return;

            int id = CurrentVolunteer.VolunteerId;
            CurrentVolunteer = null;
            CurrentVolunteer = volunteer_bl.Volunteer.GetVolunteerDetails(id);
            DataContext = null;
            DataContext = this;
            OnPropertyChanged(nameof(CurrentCallInfo));
        });
    }


}
