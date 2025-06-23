using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using BL.BIApi;
using BL.BO;
using BlApi;

namespace PL.Call;

public partial class CallWindow : Window, INotifyPropertyChanged
{
    private static readonly IBL bl = BlApi.Factory.Get();

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

    public BL.BO.Call? CurrentCall
    {
        get => (BL.BO.Call?)GetValue(CurrentCallProperty);
        set => SetValue(CurrentCallProperty, value);
    }

    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register(nameof(CurrentCall), typeof(BL.BO.Call), typeof(CallWindow), new PropertyMetadata(null));

    public CallWindow(int callId = 0)
    {
        InitializeComponent();
        ButtonText = callId != 0 ? "Update" : "Add";

        if (callId != 0)
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
        }
        else
        {
            CurrentCall = new BL.BO.Call
            {
                IdCall = 0,
                CallDescription = "",
                AddressOfCall = "",
                OpeningTime = DateTime.Now,
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
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
