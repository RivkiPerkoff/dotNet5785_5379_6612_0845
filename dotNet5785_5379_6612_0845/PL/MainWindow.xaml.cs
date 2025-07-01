using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using BL.BIApi;
using BL.BO;

namespace PL;

public partial class MainWindow : Window
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
    }
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
            }
            else
            {
                s_bl.Admin.StartSimulator(Interval);
                IsSimulatorRunning = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error toggling simulator: {ex.Message}");
        }
    }

}
