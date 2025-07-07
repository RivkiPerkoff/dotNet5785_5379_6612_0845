using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using BL.BIApi;
using BL.BO;

namespace PL;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private static readonly IBL s_bl = BlApi.Factory.Get();
    private readonly Action clockObserver;
    private readonly Action configObserver;
    private readonly int _currentUserId;
    private readonly Role _currentUserRole;

    private DispatcherOperation? _clockOperation = null;
    private DispatcherOperation? _configOperation = null;

    public MainWindow(Role currentUserRole, int currentUserId)
    {
        InitializeComponent();
        _currentUserRole = currentUserRole;
        _currentUserId = currentUserId;

        clockObserver = () =>
        {
            if (_clockOperation == null || _clockOperation.Status == DispatcherOperationStatus.Completed)
            {
                _clockOperation = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                });
            }
        };

        configObserver = () =>
        {
            if (_configOperation == null || _configOperation.Status == DispatcherOperationStatus.Completed)
            {
                _configOperation = Dispatcher.BeginInvoke(() =>
                {
                    RiskTimeRange = s_bl.Admin.GetRiskTimeRange();
                });
            }
        };

        CurrentTime = s_bl.Admin.GetClock();

        s_bl.Admin.AddClockObserver(clockObserver);
        s_bl.Admin.AddConfigObserver(configObserver);
        s_bl.Call.AddObserver(CallStatsObserver);
        CallStatsObserver();

        clockObserver();
        configObserver();
        CallList = s_bl.Call.GetFilteredAndSortedCallList();
    }
 
    public DateTime CurrentTime
    {
        get => (DateTime)GetValue(CurrentTimeProperty);
        set => SetValue(CurrentTimeProperty, value);
    }
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register(nameof(CurrentTime), typeof(DateTime), typeof(MainWindow));

    public TimeSpan RiskTimeRange
    {
        get => (TimeSpan)GetValue(RiskTimeRangeProperty);
        set => SetValue(RiskTimeRangeProperty, value);
    }
    public static readonly DependencyProperty RiskTimeRangeProperty =
        DependencyProperty.Register(nameof(RiskTimeRange), typeof(TimeSpan), typeof(MainWindow));

    public string RiskTimeRangeText
    {
        get => RiskTimeRange.ToString();
        set
        {
            if (TimeSpan.TryParse(value, out var ts))
                RiskTimeRange = ts;
            OnPropertyChanged(nameof(RiskTimeRangeText)); // עדכון ה-UI

        }
    }

    public static readonly DependencyProperty MaxRangeProperty =
        DependencyProperty.Register("MaxRange", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    public IEnumerable<CallInList> CallList
    {
        get => (IEnumerable<CallInList>)GetValue(CallListProperty);
        set => SetValue(CallListProperty, value);
    }

    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register(nameof(CallList), typeof(IEnumerable<CallInList>), typeof(MainWindow), new PropertyMetadata(null));

    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceClock(TimeUnit.Minute);
    private void btnAddOneHour_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceClock(TimeUnit.Hour);
    private void btnAddOneDay_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceClock(TimeUnit.Day);
    private void btnAddOneYear_Click(object sender, RoutedEventArgs e) => s_bl.Admin.AdvanceClock(TimeUnit.Year);

    private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
    {
        if (TimeSpan.TryParse(RiskTimeRangeText, out TimeSpan ts))
        {
            try
            {
                RiskTimeRange = ts;
                s_bl.Admin.SetRiskTimeRange(RiskTimeRange);
                MessageBox.Show($"Risk range updated to: {RiskTimeRange}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the risk range: {ex.Message}");
            }
        }
        else
        {
            MessageBox.Show("Please enter a valid time in the format hh:mm:ss.");
        }
    }

    private void btnInitializeDB_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("האם אתה בטוח שברצונך לאתחל את בסיס הנתונים?",
                                     "אישור אתחול",
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var loginWindow = new LoginWindow();
                loginWindow.Show();

                foreach (Window win in Application.Current.Windows.OfType<Window>().ToList())
                {
                    if (win != loginWindow)
                        win.Close();
                }

                s_bl.Admin.InitializeDatabase();
                MessageBox.Show("בסיס הנתונים אותחל בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }

    private void btnResetDB_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("האם אתה בטוח שברצונך לאפס את בסיס הנתונים?",
                                     "אישור איפוס",
                                     MessageBoxButton.YesNo,
                                     MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                foreach (Window win in Application.Current.Windows)
                {
                    if (win != this)
                        win.Close();
                }

                s_bl.Admin.ResetDatabase();
                MessageBox.Show("בסיס הנתונים אופס בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }

    private void btnShowVolunteerList_Click(object sender, RoutedEventArgs e)
    {
        var existingWindow = Application.Current.Windows
            .OfType<Volunteer.VolunteerListWindow>()
            .FirstOrDefault();

        if (existingWindow != null)
        {
            existingWindow.Activate();
            existingWindow.Focus();
        }
        else
        {
            var window = new Volunteer.VolunteerListWindow();
            window.Show();
        }
    }

    private void btnShowCallList_Click(object sender, RoutedEventArgs e)
    {
        var existingWindow = Application.Current.Windows
            .OfType<Call.CallListWindow>()
            .FirstOrDefault();

        if (existingWindow != null)
        {
            existingWindow.Activate();
            existingWindow.Focus();
        }
        else
        {
            var window = new Call.CallListWindow(_currentUserRole, _currentUserId);
            window.Show();
        }
    }

    private void MainWindow_Closed(object sender, EventArgs e)
    {
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
        s_bl.Call.RemoveObserver(CallStatsObserver);

        if (IsSimulatorRunning)
        {
            s_bl.Admin.StopSimulator();
            IsSimulatorRunning = false;
        }
    }
    private void UpdateCallAmounts()
    {
        try
        {
            CallAmounts = s_bl.Call.GetCallAmounts();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error retrieving call amounts: {ex.Message}");
        }
    }
    public int[] CallAmounts
    {
        get => (int[])GetValue(CallAmountsProperty);
        set => SetValue(CallAmountsProperty, value);
    }

    public static readonly DependencyProperty CallAmountsProperty =
        DependencyProperty.Register(nameof(CallAmounts), typeof(int[]), typeof(MainWindow));

    public int Interval
    {
        get => (int)GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }
    public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register(nameof(Interval), typeof(int), typeof(MainWindow), new PropertyMetadata(1));

    public bool IsSimulatorRunning
    {
        get => (bool)GetValue(IsSimulatorRunningProperty);
        set => SetValue(IsSimulatorRunningProperty, value);
    }
    public static readonly DependencyProperty IsSimulatorRunningProperty =
        DependencyProperty.Register(nameof(IsSimulatorRunning), typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

    private void ToggleSimulator_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
                EnableControls(true);
            }
            else
            {
                s_bl.Admin.StartSimulator(Interval);
                IsSimulatorRunning = true;
                EnableControls(false);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error toggling simulator: {ex.Message}");
        }
    }

    private void EnableControls(bool isEnabled)
    {
        SetButtonEnabled("+1 Minute", isEnabled);
        SetButtonEnabled("+1 Hour", isEnabled);
        SetButtonEnabled("+1 Day", isEnabled);
        SetButtonEnabled("+1 Year", isEnabled);
        SetButtonEnabled("Initialize DB", isEnabled);
        SetButtonEnabled("Reset DB", isEnabled);
        SetTextBoxEnabledByBinding(nameof(Interval), isEnabled);
    }
    private void SetButtonEnabled(string contentText, bool isEnabled)
    {
        var button = FindVisualChild<Button>(this, b => b.Content?.ToString() == contentText);
        if (button != null)
            button.IsEnabled = isEnabled;
    }

    private void SetTextBoxEnabledByBinding(string boundPropertyName, bool isEnabled)
    {
        var textBox = FindVisualChild<TextBox>(this, tb =>
        {
            var binding = BindingOperations.GetBinding(tb, TextBox.TextProperty);
            return binding?.Path?.Path == boundPropertyName;
        });

        if (textBox != null)
            textBox.IsEnabled = isEnabled;
    }

    private T? FindVisualChild<T>(DependencyObject parent, Func<T, bool> predicate) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild && predicate(typedChild))
                return typedChild;

            var result = FindVisualChild(child, predicate);
            if (result != null)
                return result;
        }
        return null;
    }
    private DispatcherOperation? _callStatsOperation = null;

    private void CallStatsObserver()
    {
        if (_callStatsOperation == null || _callStatsOperation.Status == DispatcherOperationStatus.Completed)
        {
            _callStatsOperation = Dispatcher.BeginInvoke(() =>
            {
                UpdateCallAmounts();
            });
        }
    }

    private void btnShowOpenCalls_Click(object sender, RoutedEventArgs e) =>
        ShowFilteredCallList(StatusCallType.open);

    private void btnShowOpenInRiskCalls_Click(object sender, RoutedEventArgs e) =>
        ShowFilteredCallList(StatusCallType.openInRisk);

    private void btnShowInHandlingCalls_Click(object sender, RoutedEventArgs e) =>
        ShowFilteredCallList(StatusCallType.inHandling);

    private void btnShowHandlingInRiskCalls_Click(object sender, RoutedEventArgs e) =>
        ShowFilteredCallList(StatusCallType.HandlingInRisk);

    private void btnShowClosedCalls_Click(object sender, RoutedEventArgs e) =>
        ShowFilteredCallList(StatusCallType.closed);

    private void btnShowExpiredCalls_Click(object sender, RoutedEventArgs e) =>
        ShowFilteredCallList(StatusCallType.Expired);

    private void btnShowPendingCalls_Click(object sender, RoutedEventArgs e) =>
        ShowFilteredCallList(StatusCallType.Pending);

    private void ShowFilteredCallList(StatusCallType status)
    {
        var window = new Call.CallListWindow(_currentUserRole, _currentUserId, status);
        window.Show();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
