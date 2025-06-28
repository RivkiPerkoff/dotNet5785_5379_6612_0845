//using System;
//using System.Windows;
//using System.Collections.Generic;
//using System.ComponentModel;
//using BL.BIApi;
//using BL.BO;

//namespace PL.VolunteerPersonal;

//public partial class VolunteerPersonalWindow : Window, INotifyPropertyChanged
//{
//    private static readonly IBL bl = BlApi.Factory.Get();
//    public BL.BO.Volunteer CurrentVolunteer { get; set; }
//    public IEnumerable<DistanceType> DistanceTypeCollection { get; set; }

//    public event PropertyChangedEventHandler? PropertyChanged;

//    public bool IsCallInProgress => CurrentVolunteer.CallInProgress != null;
//    public bool CanChangeActiveStatus => !IsCallInProgress;

//    public string CurrentCallInfo => CurrentVolunteer.CallInProgress == null ? "No active call" :
//        $"Address: {CurrentVolunteer.CallInProgress.CallingAddress}\nStatus: {CurrentVolunteer.CallInProgress.Status}\nDistance: {CurrentVolunteer.CallInProgress.CallingDistanceFromVolunteer} km";

//    public VolunteerPersonalWindow(int volunteerId)
//    {
//        InitializeComponent();
//        DistanceTypeCollection = Enum.GetValues(typeof(DistanceType)).Cast<DistanceType>();
//        LoadVolunteer(volunteerId);
//        DataContext = this;
//    }

//    private void LoadVolunteer(int id)
//    {
//        CurrentVolunteer = bl.Volunteer.GetVolunteerDetails(id) ?? throw new Exception("Volunteer not found");
//    }

//    private void btnUpdate_Click(object sender, RoutedEventArgs e)
//    {
//        try
//        {
//            bl.Volunteer.UpdateVolunteer(CurrentVolunteer.VolunteerId, CurrentVolunteer);
//            MessageBox.Show("Details updated successfully.");
//            RefreshBindings();
//        }
//        catch (Exception ex)
//        {
//            MessageBox.Show($"Error: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
//        }
//    }

//    private void btnEndCall_Click(object sender, RoutedEventArgs e)
//    {
//        try
//        {
//            if (CurrentVolunteer.CallInProgress == null) return;
//            bl.Call.CompleteCallTreatment(CurrentVolunteer.VolunteerId, CurrentVolunteer.CallInProgress.Id);
//            MessageBox.Show("Call ended.");
//            LoadVolunteer(CurrentVolunteer.VolunteerId);
//            RefreshBindings();
//        }
//        catch (Exception ex)
//        {
//            MessageBox.Show($"Error: {ex.Message}");
//        }
//    }

//    private void btnCancelCall_Click(object sender, RoutedEventArgs e)
//    {
//        try
//        {
//            if (CurrentVolunteer.CallInProgress == null) return;
//            bl.Call.CancelCallTreatment(CurrentVolunteer.VolunteerId, CurrentVolunteer.CallInProgress.Id);
//            MessageBox.Show("Call canceled.");
//            LoadVolunteer(CurrentVolunteer.VolunteerId);
//            RefreshBindings();
//        }
//        catch (Exception ex)
//        {
//            MessageBox.Show($"Error: {ex.Message}");
//        }
//    }

//    private void btnHistory_Click(object sender, RoutedEventArgs e)
//    {
//        new Call.CallListWindow().ShowDialog();
//    }

//    private void RefreshBindings()
//    {
//        OnPropertyChanged(nameof(CurrentCallInfo));
//        OnPropertyChanged(nameof(IsCallInProgress));
//        OnPropertyChanged(nameof(CanChangeActiveStatus));
//    }

//    protected virtual void OnPropertyChanged(string propertyName) =>
//        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    private void Window_Loaded(object sender, RoutedEventArgs e)
//    {
//        RefreshBindings();
//    }

//}

using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using BL.BIApi;
using BL.BO;

namespace PL.VolunteerPersonal;

public partial class VolunteerPersonalWindow : Window, INotifyPropertyChanged
{
    private static readonly IBL bl = BlApi.Factory.Get();

    public BL.BO.Volunteer CurrentVolunteer { get; set; } = null!;
    public IEnumerable<DistanceType> DistanceTypeCollection { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsCallInProgress => CurrentVolunteer.CallInProgress != null;

    public bool CanChangeActiveStatus => !IsCallInProgress;

    public Visibility ChooseCallButtonVisibility => IsCallInProgress ? Visibility.Collapsed : Visibility.Visible;

    public string CurrentCallInfo => CurrentVolunteer.CallInProgress == null ? "No active call" :
        $"Address: {CurrentVolunteer.CallInProgress.CallingAddress}\n" +
        $"Status: {CurrentVolunteer.CallInProgress.Status}\n" +
        $"Distance: {CurrentVolunteer.CallInProgress.CallingDistanceFromVolunteer} km";

    public VolunteerPersonalWindow(int volunteerId)
    {
        InitializeComponent();
        DistanceTypeCollection = Enum.GetValues(typeof(DistanceType)).Cast<DistanceType>();
        LoadVolunteer(volunteerId);
        DataContext = this;
    }

    private void LoadVolunteer(int id)
    {
        CurrentVolunteer = bl.Volunteer.GetVolunteerDetails(id) ?? throw new Exception("Volunteer not found");
    }

    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bl.Volunteer.UpdateVolunteer(CurrentVolunteer.VolunteerId, CurrentVolunteer);
            MessageBox.Show("Details updated successfully.");
            LoadVolunteer(CurrentVolunteer.VolunteerId);
            RefreshBindings();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnEndCall_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentVolunteer.CallInProgress == null) return;
            bl.Call.CompleteCallTreatment(CurrentVolunteer.VolunteerId, CurrentVolunteer.CallInProgress.Id);
            MessageBox.Show("Call ended.");
            LoadVolunteer(CurrentVolunteer.VolunteerId);
            RefreshBindings();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private void btnCancelCall_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentVolunteer.CallInProgress == null) return;
            bl.Call.CancelCallTreatment(CurrentVolunteer.VolunteerId, CurrentVolunteer.CallInProgress.Id);
            MessageBox.Show("Call canceled.");
            LoadVolunteer(CurrentVolunteer.VolunteerId);
            RefreshBindings();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
    }

    private void btnChooseCall_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Here you would open the Choose Call window...");
        // כאן תוכל לפתוח את המסך האמיתי של בחירת קריאה
    }

    private void btnHistory_Click(object sender, RoutedEventArgs e)
    {
        var historyWindow = new VolunteerCallHistoryWindow(CurrentVolunteer.VolunteerId);
        historyWindow.ShowDialog();
    }

    private void btnAllCalls_Click(object sender, RoutedEventArgs e)
    {
        new Call.CallListWindow().ShowDialog();
    }

    private void RefreshBindings()
    {
        OnPropertyChanged(nameof(ChooseCallButtonVisibility));
        OnPropertyChanged(nameof(CurrentCallInfo));
        OnPropertyChanged(nameof(IsCallInProgress));
        OnPropertyChanged(nameof(CanChangeActiveStatus));
    }

    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        RefreshBindings();
    }



}

