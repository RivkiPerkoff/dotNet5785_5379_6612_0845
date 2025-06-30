using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using BL.BIApi;
using BL.BO;
using System.Windows.Controls;
using PL.Call;

namespace PL.VolunteerPersonal
{
    public partial class VolunteerPersonalWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBL bl = BlApi.Factory.Get();

        private BL.BO.Volunteer _currentVolunteer = null!;
        public BL.BO.Volunteer CurrentVolunteer
        {
            get => _currentVolunteer;
            set
            {
                _currentVolunteer = value;
                OnPropertyChanged(nameof(CurrentVolunteer));
                OnPropertyChanged(nameof(IsCallInProgress));
                OnPropertyChanged(nameof(CanChooseCall));
                OnPropertyChanged(nameof(CanChangeActiveStatus));
                OnPropertyChanged(nameof(CurrentCallInfo));
            }
        }

        public IEnumerable<DistanceType> DistanceTypeCollection { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsCallInProgress => CurrentVolunteer.CallInProgress != null;
        public bool CanChooseCall => CurrentVolunteer.IsAvailable && CurrentVolunteer.CallInProgress == null;
        public bool CanChangeActiveStatus => CurrentVolunteer.CallInProgress == null;

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
            PasswordBoxVolunteer.Password = "";
            OnPropertyChanged(nameof(CurrentCallInfo));
            OnPropertyChanged(nameof(IsCallInProgress));
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentVolunteer.Name) ||
                    string.IsNullOrWhiteSpace(CurrentVolunteer.PhoneNumber) ||
                    string.IsNullOrWhiteSpace(CurrentVolunteer.AddressVolunteer) ||
                    CurrentVolunteer.MaximumDistanceForReceivingCall <= 0)
                {
                    MessageBox.Show("All mandatory fields must be filled.");
                    return;
                }
                // ולידציה לפורמט טלפון
                if (!System.Text.RegularExpressions.Regex.IsMatch(CurrentVolunteer.PhoneNumber, @"^\d{9,10}$"))
                {
                    MessageBox.Show("Phone number must contain 9-10 digits.");
                    return;
                }

                // ולידציה לפורמט אימייל - רק אם הוזן (כי זה לא שדה חובה)
                if (!string.IsNullOrWhiteSpace(CurrentVolunteer.EmailOfVolunteer) &&
                    !System.Text.RegularExpressions.Regex.IsMatch(CurrentVolunteer.EmailOfVolunteer, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show("Invalid email format.");
                    return;
                }

                var existingVolunteer = bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.VolunteerId);

                // חובה להזין סיסמה לצורך עדכון
                if (string.IsNullOrWhiteSpace(PasswordBoxVolunteer.Password))
                {
                    MessageBox.Show("Password must be entered to update details.", "Missing Password", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                CurrentVolunteer.PasswordVolunteer = PasswordBoxVolunteer.Password;


                // שמירת מייל אם לא הוזן חדש (למניעת מחיקה בטעות)
                if (string.IsNullOrWhiteSpace(CurrentVolunteer.EmailOfVolunteer))
                    CurrentVolunteer.EmailOfVolunteer = existingVolunteer.EmailOfVolunteer;

                bl.Volunteer.UpdateVolunteer(CurrentVolunteer.VolunteerId, CurrentVolunteer);
                MessageBox.Show("Details updated successfully.");

                LoadVolunteer(CurrentVolunteer.VolunteerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //private void btnChooseCall_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(CurrentVolunteer.AddressVolunteer))
        //        {
        //            MessageBox.Show("Cannot choose a call - your address is missing or invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        //            return;
        //        }
        //        var chooseCallWindow = new PL.Call.ChooseCallWindow(CurrentVolunteer);
        //        //chooseCallWindow.Closed += (s, args) =>
        //        //{
        //        //    LoadVolunteer(CurrentVolunteer.VolunteerId);
        //        //};
        //        chooseCallWindow.Closed += (s, args) =>
        //        {
        //            if (chooseCallWindow.SelectedCallInProgress != null)
        //            {
        //                CurrentVolunteer.CallInProgress = chooseCallWindow.SelectedCallInProgress;
        //                OnPropertyChanged(nameof(CurrentVolunteer));
        //                OnPropertyChanged(nameof(IsCallInProgress));
        //                OnPropertyChanged(nameof(CanChooseCall));
        //                OnPropertyChanged(nameof(CanChangeActiveStatus));
        //                OnPropertyChanged(nameof(CurrentCallInfo));
        //            }
        //            else
        //            {
        //                LoadVolunteer(CurrentVolunteer.VolunteerId);
        //            }
        //        };
        //        chooseCallWindow.ShowDialog();

        //        //LoadVolunteer(CurrentVolunteer.VolunteerId);

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error opening Choose Call window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
        private void btnChooseCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CurrentVolunteer.AddressVolunteer))
                {
                    MessageBox.Show("Cannot choose a call - your address is missing or invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var chooseCallWindow = new PL.Call.ChooseCallWindow(CurrentVolunteer);
                chooseCallWindow.Closed += (s, args) =>
                {
                    LoadVolunteer(CurrentVolunteer.VolunteerId);
                };

                chooseCallWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Choose Call window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerCallHistoryWindow(CurrentVolunteer.VolunteerId).ShowDialog();
        }

        private void btnEndCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentVolunteer.CallInProgress == null)
                    return;

                bl.Call.CompleteCallTreatment(CurrentVolunteer.VolunteerId, CurrentVolunteer.CallInProgress.Id);
                MessageBox.Show("Call ended.");
                LoadVolunteer(CurrentVolunteer.VolunteerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error ending call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentVolunteer.CallInProgress == null)
                    return;

                bl.Call.CancelCallTreatment(CurrentVolunteer.VolunteerId, CurrentVolunteer.CallInProgress.Id);
                MessageBox.Show("Call canceled.");
                LoadVolunteer(CurrentVolunteer.VolunteerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error canceling call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //private void ActiveChanged(object sender, RoutedEventArgs e)
        //{
        //    OnPropertyChanged(nameof(CanChooseCall));
        //    OnPropertyChanged(nameof(CanChangeActiveStatus));
        //}
        private void ActiveChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer.CallInProgress != null)
            {
                MessageBox.Show("Cannot change availability while handling a call.");
                // מחזיר את ה־Checkbox למצבו המקורי
                ((CheckBox)sender).IsChecked = CurrentVolunteer.IsAvailable;
                return;
            }

            OnPropertyChanged(nameof(CanChooseCall));
            OnPropertyChanged(nameof(CanChangeActiveStatus));
        }


        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}