using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BL.BIApi;
using BL.BO;

namespace PL.Call;

public partial class CallListWindow : Window
{
    private static readonly IBL s_bl = BlApi.Factory.Get();

    public CallListWindow()
    {
        InitializeComponent();
        DataContext = this;

        SortFields = Enum.GetValues(typeof(CallInListFields)).Cast<CallInListFields>().ToList();
        RefreshCallList();
        s_bl.Call.AddObserver(RefreshCallList);
    }

    // Properties
    public CallInList? SelectedCall { get; set; }

    public List<CallInListFields> SortFields { get; set; }

    private CallInListFields _selectedSortField = CallInListFields.CallId;
    public CallInListFields SelectedSortField
    {
        get => _selectedSortField;
        set
        {
            if (_selectedSortField != value)
            {
                _selectedSortField = value;
                RefreshCallList();
            }
        }
    }

    private StatusCallType? _selectedStatusFilter = null;
    public StatusCallType? SelectedStatusFilter
    {
        get => _selectedStatusFilter;
        set
        {
            if (_selectedStatusFilter != value)
            {
                _selectedStatusFilter = value;
                RefreshCallList();
            }
        }
    }

    public IEnumerable<CallInList> CallList
    {
        get => (IEnumerable<CallInList>)GetValue(CallListProperty);
        set => SetValue(CallListProperty, value);
    }

    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register(nameof(CallList), typeof(IEnumerable<CallInList>), typeof(CallListWindow));

    // Refresh List
    private void RefreshCallList()
    {
        CallList = s_bl.Call.GetFilteredAndSortedCallList(
            filterBy: SelectedStatusFilter != null ? CallInListFields.Status : null,
            filterValue: SelectedStatusFilter,
            sortBy: SelectedSortField
        );
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(CallListObserver);
        RefreshCallList();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(CallListObserver);
    }

    private void CallListObserver() => RefreshCallList();

    // Add call
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var win = new CallWindow();
        win.ShowDialog();
    }

    // Edit call
    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall != null)
        {
            var win = new CallWindow(SelectedCall.CallId);
            win.ShowDialog();
        }
    }

    // Delete call
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int callId)
        {
            var call = CallList?.FirstOrDefault(c => c.CallId == callId);
            if (call == null)
                return;

            var result = MessageBox.Show($"Are you sure you want to delete call #{call.CallId}?",
                                         "Confirm Deletion",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                s_bl.Call.DeleteCall(callId);
                MessageBox.Show("Call deleted successfully.");
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += $"\nInner: {ex.InnerException.Message}";
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
