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

    public bool CanChooseCall => CurrentVolunteer.IsAvailable && CurrentVolunteer.CallInProgress == null;


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
        if (string.IsNullOrWhiteSpace(CurrentVolunteer.Name))
        {
            MessageBox.Show("Name is required.");
            return;
        }
        if (string.IsNullOrWhiteSpace(CurrentVolunteer.PhoneNumber))
        {
            MessageBox.Show("Phone number is required.");
            return;
        }
        if (string.IsNullOrWhiteSpace(CurrentVolunteer.AddressVolunteer))
        {
            MessageBox.Show("Address is required.");
            return;
        }
        if (CurrentVolunteer.MaximumDistanceForReceivingCall <= 0)
        {
            MessageBox.Show("Maximum distance must be greater than 0.");
            return;
        }

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
        //catch (Exception ex)
        //{
        //    MessageBox.Show($"Error: {ex.Message}");
        //}
        catch (Exception ex)
        {
            string fullMessage = ex.Message;
            if (ex.InnerException != null)
                fullMessage += "\nDetails: " + ex.InnerException.Message;

            MessageBox.Show("Error loading calls: " + fullMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        //catch (Exception ex)
        //{
        //    MessageBox.Show($"Error: {ex.Message}");
        //}
        catch (Exception ex)
        {
            string fullMessage = ex.Message;
            if (ex.InnerException != null)
                fullMessage += "\nDetails: " + ex.InnerException.Message;

            MessageBox.Show("Error loading calls: " + fullMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    //private void btnChooseCall_Click(object sender, RoutedEventArgs e)
    //{
    //    var chooseCallWindow = new PL.Call.ChooseCallWindow(CurrentVolunteer);
    //    if (chooseCallWindow.ShowDialog() == true)
    //    {
    //        LoadVolunteer(CurrentVolunteer.VolunteerId);
    //        RefreshBindings();
    //        //DrawMap();
    //    }
    //}
    //private void btnChooseCall_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        var chooseCallWindow = new PL.Call.ChooseCallWindow(CurrentVolunteer);
    //        if (chooseCallWindow.ShowDialog() == true)
    //        {
    //            LoadVolunteer(CurrentVolunteer.VolunteerId);
    //            RefreshBindings();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        string fullMessage = ex.Message;
    //        if (ex.InnerException != null)
    //            fullMessage += "\nDetails: " + ex.InnerException.Message;

    //        MessageBox.Show("Error opening Choose Call window: " + fullMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //    }
    //}
    //private void btnChooseCall_Click(object sender, RoutedEventArgs e)
    //{
    //    try
    //    {
    //        if (CurrentVolunteer == null)
    //        {
    //            MessageBox.Show("Volunteer data is not loaded properly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //            return;
    //        }
    //        if (string.IsNullOrWhiteSpace(CurrentVolunteer.AddressVolunteer))
    //        {
    //            MessageBox.Show("Cannot choose a call - your address is missing or invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    //            return;
    //        }

    //        var chooseCallWindow = new PL.Call.ChooseCallWindow(CurrentVolunteer);
    //        if (chooseCallWindow.ShowDialog() == true)
    //        {
    //            LoadVolunteer(CurrentVolunteer.VolunteerId);
    //            RefreshBindings();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        string fullMessage = ex.Message;
    //        if (ex.InnerException != null)
    //            fullMessage += "\nDetails: " + ex.InnerException.Message;

    //        MessageBox.Show("Error opening Choose Call window: " + fullMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //    }
    //}
    private void btnChooseCall_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentVolunteer == null)
            {
                MessageBox.Show("Volunteer data is not loaded properly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentVolunteer.AddressVolunteer))
            {
                MessageBox.Show("Cannot choose a call - your address is missing or invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PL.Call.ChooseCallWindow chooseCallWindow;
            try
            {
                chooseCallWindow = new PL.Call.ChooseCallWindow(CurrentVolunteer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating Choose Call window:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (chooseCallWindow.ShowDialog() == true)
            {
                CurrentVolunteer = chooseCallWindow.GetUpdatedVolunteer();
                LoadVolunteer(CurrentVolunteer.VolunteerId);
                RefreshBindings();
            }
        }
        catch (Exception ex)
        {
            string fullMessage = ex.Message;
            if (ex.InnerException != null)
                fullMessage += "\nDetails: " + ex.InnerException.Message;

            MessageBox.Show("Error opening Choose Call window: " + fullMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void btnHistory_Click(object sender, RoutedEventArgs e)
    {
        var historyWindow = new VolunteerCallHistoryWindow(CurrentVolunteer.VolunteerId);
        historyWindow.ShowDialog();
    }

  
    private void RefreshBindings()
    {
        OnPropertyChanged(nameof(CanChooseCall));
        OnPropertyChanged(nameof(CurrentCallInfo));
        OnPropertyChanged(nameof(IsCallInProgress));

    }

    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        LoadVolunteer(CurrentVolunteer.VolunteerId);
        RefreshBindings();
    }
}

