using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using BL.BIApi;
using BL.BO;

namespace PL.Call;

public partial class CallListWindow : Window
{
    private static readonly IBL s_bl = BlApi.Factory.Get();
    private readonly Role _currentUserRole;
    private readonly int _currentUserId;

    private readonly Action _clockObserver;
    private volatile DispatcherOperation? _callListRefreshOperation = null;
    private volatile DispatcherOperation? _observerRefreshOperation = null;

    public CallListWindow(Role currentUserRole, int currentUserId)
    {
        InitializeComponent();
        _currentUserRole = currentUserRole;
        _currentUserId = currentUserId;

        DataContext = this;
        SortFields = Enum.GetValues(typeof(CallInListFields)).Cast<CallInListFields>().ToList();

        _clockObserver = () =>
        {
            if (_observerRefreshOperation == null || _observerRefreshOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerRefreshOperation = Dispatcher.BeginInvoke(() =>
                {
                    RefreshCallList();
                });
            }
        };

        RefreshCallList();
        s_bl.Call.AddObserver(RefreshCallList);
        s_bl.Admin.AddClockObserver(_clockObserver);
    }

    public bool IsManager => _currentUserRole == Role.Manager;

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

    private void RefreshCallList()
    {
        if (_callListRefreshOperation != null && _callListRefreshOperation.Status != DispatcherOperationStatus.Completed)
            return;

        _callListRefreshOperation = Dispatcher.BeginInvoke(new Action(() =>
        {
            CallList = s_bl.Call.GetFilteredAndSortedCallList(
                filterBy: SelectedStatusFilter != null ? CallInListFields.Status : null,
                filterValue: SelectedStatusFilter,
                sortBy: SelectedSortField
            );
        }), DispatcherPriority.Background);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(CallListObserver);
        RefreshCallList();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(CallListObserver);
        s_bl.Admin.RemoveClockObserver(_clockObserver);
    }

    private void CallListObserver()
    {
        if (_observerRefreshOperation != null && _observerRefreshOperation.Status != DispatcherOperationStatus.Completed)
            return;

        _observerRefreshOperation = Dispatcher.BeginInvoke(new Action(() =>
        {
            RefreshCallList();
        }), DispatcherPriority.Background);
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var win = new CallWindow();
        win.ShowDialog();
        RefreshCallList();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall != null)
        {
            var win = new CallWindow(SelectedCall.CallId);
            win.ShowDialog();
            RefreshCallList();
        }
    }

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
                RefreshCallList();
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

    private void DeleteAssignmentButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is int callId)
        {
            var call = CallList?.FirstOrDefault(c => c.CallId == callId);
            if (call == null)
                return;

            if (call.Status != StatusCallType.inHandling && call.Status != StatusCallType.HandlingInRisk)
            {
                MessageBox.Show("An assignment can only be deleted when the call is in handling or handling at risk.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete the assignment for call #{call.CallId}?",
                                         "Confirm Deletion",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var assignments = s_bl.Call.GetCallDetails(callId).CallAssignInLists;
                var activeAssignment = assignments?
                    .Where(a => a.EndTimeForTreatment == null)
                    .OrderByDescending(a => a.EntryTimeForTreatment)
                    .FirstOrDefault();

                if (activeAssignment == null)
                    throw new Exception("No active assignment found for deletion.");

                s_bl.Call.CancelCallTreatment(_currentUserId, call.Id.Value);

                MessageBox.Show("Assignment successfully canceled.");
                RefreshCallList();
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
