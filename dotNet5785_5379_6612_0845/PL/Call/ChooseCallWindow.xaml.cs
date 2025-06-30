//using System.Collections.Generic;
//using System.Windows;
//using System.Windows.Controls;

//namespace PL.Volunteer
//{
//    public partial class ChooseCallWindow : Window
//    {
//        private readonly BL.BO.Volunteer _volunteer;
//        private readonly BL.BIApi.IBL _bl = BlApi.Factory.Get();

//        public IEnumerable<BL.BO.OpenCallInList> CallsList { get; set; } = new List<BL.BO.OpenCallInList>();

//        public ChooseCallWindow(BL.BO.Volunteer volunteer)
//        {
//            InitializeComponent();
//            _volunteer = volunteer;
//            LoadCalls();
//            DataContext = this;
//        }

//        private void LoadCalls()
//        {
//            try
//            {
//                CallsList = _bl.Call.GetOpenCallsForVolunteerSelection(_volunteer.VolunteerId, null, null);
//            }
//            catch (System.Exception ex)
//            {
//                MessageBox.Show("Error loading calls: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void ChooseCall_Click(object sender, RoutedEventArgs e)
//        {
//            if (sender is Button btn && btn.DataContext is BL.BO.OpenCallInList selectedCall)
//            {
//                try
//                {
//                    _bl.Call.ChoosingCallForTreatment(_volunteer.VolunteerId, selectedCall.Id);
//                    MessageBox.Show("Call successfully assigned to you.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
//                    LoadCalls();
//                }
//                catch (System.Exception ex)
//                {
//                    MessageBox.Show("Error assigning call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//        }

//        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (((DataGrid)sender).SelectedItem is BL.BO.OpenCallInList selectedCall)
//            {
//                txtDescription.Text = selectedCall.CallDescription;
//            }
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BL.BO;

namespace PL.Call;

public partial class ChooseCallWindow : Window, INotifyPropertyChanged
{
    private readonly BL.BO.Volunteer _volunteer;
    private readonly BL.BIApi.IBL _bl = BlApi.Factory.Get();

    public IEnumerable<OpenCallInList> CallsList { get; set; } = new List<OpenCallInList>();
    public IEnumerable<CallTypes> CallTypeOptions { get; set; }
    public IEnumerable<OpenCallInListFields> SortFieldOptions { get; set; }

    public CallTypes? SelectedFilterType { get; set; }
    public OpenCallInListFields? SelectedSortField { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ChooseCallWindow(BL.BO.Volunteer volunteer)
    {
        InitializeComponent();
        _volunteer = volunteer;

        CallTypeOptions = Enum.GetValues(typeof(CallTypes)) as CallTypes[];
        SortFieldOptions = Enum.GetValues(typeof(OpenCallInListFields)) as OpenCallInListFields[];

        LoadCalls();

        DataContext = this;
    }

    //private void LoadCalls()
    //{
    //    try
    //    {
    //        CallsList = _bl.Call.GetOpenCallsForVolunteerSelection(
    //            _volunteer.VolunteerId,
    //            SelectedFilterType,
    //            SelectedSortField
    //        );
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show("Error loading calls: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    //    }
    //}

    private void LoadCalls()
    {
        try
        {
            var allCalls = _bl.Call.GetOpenCallsForVolunteerSelection(
                _volunteer.VolunteerId,
                SelectedFilterType,
                SelectedSortField
            );

            if (_volunteer.MaximumDistanceForReceivingCall.HasValue)
            {
                CallsList = allCalls
                    .Where(c => c.CallDistance <= _volunteer.MaximumDistanceForReceivingCall.Value)
                    .ToList();
            }
            else
            {
                CallsList = allCalls.ToList();
            }

            OnPropertyChanged(nameof(CallsList));
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading calls: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ChooseCall_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is OpenCallInList selectedCall)
        {
            try
            {
                _bl.Call.ChoosingCallForTreatment(_volunteer.VolunteerId, selectedCall.Id);
                MessageBox.Show("Call successfully assigned to you.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadCalls();
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error assigning call: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (((DataGrid)sender).SelectedItem is OpenCallInList selectedCall)
        {
            SelectedCallDescription = selectedCall.CallDescription;
        }
    }


    private void Filter_Changed(object sender, SelectionChangedEventArgs e)
    {
        LoadCalls();
    }
    private void DataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var row = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
        if (row != null)
        {
            if (row.Item is OpenCallInList selectedCall)
            {
                SelectedCallDescription = selectedCall.CallDescription;
                //ShowMap(selectedCall);
            }
        }
    }

    private static T? FindVisualParent<T>(DependencyObject? child) where T : DependencyObject
    {
        while (child != null)
        {
            if (child is T parent)
                return parent;

            child = VisualTreeHelper.GetParent(child);
        }
        return null;
    }
    public string SelectedCallDescription
    {
        get => _selectedCallDescription;
        set
        {
            _selectedCallDescription = value;
            OnPropertyChanged(nameof(SelectedCallDescription));
        }
    }
    private string _selectedCallDescription = string.Empty;


    //private void ShowMap(OpenCallInList call)
    //{
    //    try
    //    {
    //        string mapsUrl = $"https://www.google.com/maps/dir/{_volunteer.AddressVolunteer}/{call.Address}";
    //        mapBrowser.Navigate(mapsUrl);
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show("Error loading map: " + ex.Message);
    //    }
    //}

    //private void UpdateAddress_Click(object sender, RoutedEventArgs e)
    //{
    //    var newAddress = Microsoft.VisualBasic.Interaction.InputBox("Enter new address:", "Update Address");
    //    if (!string.IsNullOrWhiteSpace(newAddress))
    //    {
    //        _volunteer.AddressVolunteer = newAddress;
    //        LoadCalls();
    //    }
    //}
    private void UpdateAddress_Click(object sender, RoutedEventArgs e)
    {
        var newAddress = Microsoft.VisualBasic.Interaction.InputBox("Enter new address:", "Update Address");

        if (!string.IsNullOrWhiteSpace(newAddress))
        {
            try
            {
                _volunteer.AddressVolunteer = newAddress;
                _bl.Volunteer.UpdateVolunteer(_volunteer.VolunteerId, _volunteer);
                MessageBox.Show("Address updated successfully.");
                LoadCalls();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating address: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
