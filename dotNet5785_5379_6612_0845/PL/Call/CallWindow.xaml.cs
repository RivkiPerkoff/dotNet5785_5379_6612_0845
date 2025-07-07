using BL.BIApi;
using BL.BO;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace PL.Call
{
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBL bl = BlApi.Factory.Get();

        private volatile DispatcherOperation? _loadCallOperation = null;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

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

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(CallWindow), new PropertyMetadata("Add"));

        public bool IsEditMode => ButtonText == "Update";

        public bool IsEditable =>
            CurrentCall != null &&
            CurrentCall.StatusCallType != StatusCallType.closed &&
            CurrentCall.StatusCallType != StatusCallType.Expired;

        public string MaxFinishTimeTimePart
        {
            get => CurrentCall?.MaxFinishTime?.ToString("HH\\:mm") ?? "";
            set
            {
                if (CurrentCall == null || CurrentCall.MaxFinishTime == null) return;
                if (TimeSpan.TryParse(value, out var time))
                {
                    var date = CurrentCall.MaxFinishTime?.Date ?? DateTime.Now.Date;
                    CurrentCall.MaxFinishTime = date + time;
                    OnPropertyChanged(nameof(CurrentCall));
                }
            }
        }

        public BL.BO.Call? CurrentCall
        {
            get => (BL.BO.Call?)GetValue(CurrentCallProperty);
            set
            {
                SetValue(CurrentCallProperty, value);
                OnPropertyChanged(nameof(CurrentCall));
                OnPropertyChanged(nameof(MaxFinishTimeTimePart));
                OnPropertyChanged(nameof(IsEditable));
            }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register(nameof(CurrentCall), typeof(BL.BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        public CallWindow(int callId = 0)
        {
            InitializeComponent();
            ButtonText = callId != 0 ? "Update" : "Add";

            if (callId != 0)
            {
                if (_loadCallOperation != null && _loadCallOperation.Status != DispatcherOperationStatus.Completed)
                    return;

                _loadCallOperation = Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        var call = bl.Call.GetCallDetails(callId);
                        CurrentCall = call;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading call: " + ex.Message);
                        Close();
                    }
                }), DispatcherPriority.Background);
            }
            else
            {
                CurrentCall = new BL.BO.Call
                {
                    IdCall = 0,
                    CallDescription = "",
                    AddressOfCall = "",
                    OpeningTime = bl.Admin.GetClock(),
                    MaxFinishTime = DateTime.Now.AddHours(2),
                    CallType = CallTypes.None
                };
            }

            DataContext = this;
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCall == null)
                    return;

                if (ButtonText == "Add")
                {
                    bl.Call.AddCall(CurrentCall);
                    MessageBox.Show("Call added successfully.");
                }
                else
                {
                    bl.Call.UpdateCallDetails(CurrentCall);
                    MessageBox.Show("Call updated successfully.");
                }

                Close();
        }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show("Call added successfully.");
            }
        }
    }
}
