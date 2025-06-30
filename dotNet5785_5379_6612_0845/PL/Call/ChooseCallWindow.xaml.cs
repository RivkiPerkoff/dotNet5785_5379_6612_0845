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
        private readonly BL.BIApi.IBL _bl = BlApi.Factory.Get();
        public BL.BO.Volunteer GetUpdatedVolunteer() => _volunteer;

        public IEnumerable<OpenCallInList> CallsList { get; set; } = new List<OpenCallInList>();
        public IEnumerable<CallTypes> CallTypeOptions { get; set; }
        public IEnumerable<OpenCallInListFields> SortFieldOptions { get; set; }

        public CallTypes? SelectedFilterType { get; set; }
        public OpenCallInListFields? SelectedSortField { get; set; }

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

        public ChooseCallWindow(BL.BO.Volunteer volunteer)
        {
            InitializeComponent();
            _volunteer = volunteer;

            CallTypeOptions = Enum.GetValues(typeof(CallTypes)).Cast<CallTypes>().ToList();
            SortFieldOptions = Enum.GetValues(typeof(OpenCallInListFields)).Cast<OpenCallInListFields>().ToList();

            DataContext = this;
            LoadCalls();
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

                // סינון לפי מרחק מקסימלי של המתנדב
                if (_volunteer.MaximumDistanceForReceivingCall.HasValue && _volunteer.MaximumDistanceForReceivingCall > 0)
                {
                    CallsList = allCalls
                        .Where(c => c.CallDistance <= _volunteer.MaximumDistanceForReceivingCall.Value)
                        .ToList();
                }
                else
                {
                    // אם לא מוגדר מרחק, מציגים את כל הקריאות
                    CallsList = allCalls.ToList();
                }

                OnPropertyChanged(nameof(CallsList));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading calls: " + ex.Message);
            }
        }
        public CallInProgress? SelectedCallInProgress { get; private set; }

        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is OpenCallInList selectedCall)
            {
                try
                {
                    _bl.Call.ChoosingCallForTreatment(_volunteer.VolunteerId, selectedCall.Id);
                    SelectedCallInProgress = _bl.Volunteer.GetVolunteerDetails(_volunteer.VolunteerId).CallInProgress;

                    MessageBox.Show("Call assigned to you.");
                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error assigning call: " + ex.Message);
                }
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((DataGrid)sender).SelectedItem is OpenCallInList selectedCall)
                SelectedCallDescription = selectedCall.CallDescription;
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadCalls();
        }

        private void DataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
            if (row != null && row.Item is OpenCallInList selectedCall)
                SelectedCallDescription = selectedCall.CallDescription;
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
                    MessageBox.Show("Address updated.");
                    LoadCalls();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating address: " + ex.Message);
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
