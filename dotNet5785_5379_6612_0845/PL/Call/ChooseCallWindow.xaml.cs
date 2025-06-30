using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BL.BIApi;
using BL.BO;

namespace PL.Call
{
    public partial class ChooseCallWindow : Window, INotifyPropertyChanged
    {
        private readonly BL.BO.Volunteer _volunteer;
        private readonly IBL _bl = BlApi.Factory.Get();

        public IEnumerable<OpenCallInList> CallsList { get; set; } = new List<OpenCallInList>();
        public IEnumerable<CallTypes> CallTypeOptions { get; set; }
        public IEnumerable<OpenCallInListFields> SortFieldOptions { get; set; }

        private CallTypes? _selectedFilterType;
        public CallTypes? SelectedFilterType
        {
            get => _selectedFilterType;
            set
            {
                _selectedFilterType = value;
                OnPropertyChanged(nameof(SelectedFilterType));
            }
        }

        private OpenCallInListFields? _selectedSortField;
        public OpenCallInListFields? SelectedSortField
        {
            get => _selectedSortField;
            set
            {
                _selectedSortField = value;
                OnPropertyChanged(nameof(SelectedSortField));
            }
        }

        private string _selectedCallDescription = string.Empty;
        public string SelectedCallDescription
        {
            get => _selectedCallDescription;
            set
            {
                _selectedCallDescription = value;
                OnPropertyChanged(nameof(SelectedCallDescription));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ChooseCallWindow(BL.BO.Volunteer volunteer)
        {
            InitializeComponent();
            _volunteer = volunteer;

            CallTypeOptions = Enum.GetValues(typeof(CallTypes)).Cast<CallTypes>();
            SortFieldOptions = Enum.GetValues(typeof(OpenCallInListFields)).Cast<OpenCallInListFields>();

            LoadCalls();
            DataContext = this;
        }

        private void LoadCalls()
        {
            try
            {
                var allCalls = _bl.Call.GetOpenCallsForVolunteerSelection(
                    _volunteer.VolunteerId,
                    SelectedFilterType,
                    SelectedSortField
                );

                CallsList = _volunteer.MaximumDistanceForReceivingCall.HasValue
                    ? allCalls.Where(c => c.CallDistance <= _volunteer.MaximumDistanceForReceivingCall.Value).ToList()
                    : allCalls.ToList();

                OnPropertyChanged(nameof(CallsList));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading calls: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ✅ זו המתודה שחסרה וגרמה לשגיאה
        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadCalls();
        }

        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is OpenCallInList selectedCall)
            {
                try
                {
                    _bl.Call.ChoosingCallForTreatment(_volunteer.VolunteerId, selectedCall.Id);
                    MessageBox.Show("Call successfully assigned to you.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void DataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
            if (row?.Item is OpenCallInList selectedCall)
            {
                SelectedCallDescription = selectedCall.CallDescription;
            }
        }

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
    }
}
