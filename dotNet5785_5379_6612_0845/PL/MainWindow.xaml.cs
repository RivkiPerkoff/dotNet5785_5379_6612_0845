using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BL.BIApi;

namespace PL;
public partial class MainWindow : Window
{
    private static readonly IBL s_bl = BlApi.Factory.Get();
    // משתנים לשמירת המשקיפים
    private readonly Action clockObserver;
    private readonly Action configObserver;

    public MainWindow()
    {
        InitializeComponent();

        clockObserver = () =>
        {
            CurrentTime = s_bl.Admin.GetClock();
        };

        //configObserver = () =>
        //{
        //    //// במקום לחשב שנים מתוך ימים, קבל את הטווח ישירות כ-TimeSpan
        //    //var riskTimeRange = s_bl.Admin.GetRiskTimeRange();
        //    //// אם ברצונך לשמור כערך מספרי בשנים (או שעות) אפשר להוסיף כאן המרה בהתאם

        //    //// לדוגמה, אם MaxYearRange צריך להיות מספר של שעות:
        //    //MaxYearRange = (int)riskTimeRange.TotalHours;

        //    //// אם עדיין רוצה לשמור בשנים, אז תעשה המרה:
        //    //// MaxYearRange = riskTimeRange.Days / 365;
        //    ///
        //};

        configObserver = () =>
        {
            RiskTimeRange = s_bl.Admin.GetRiskTimeRange();
            // אם אתה לא רוצה לחשב שנים בכלל, פשוט אל תשתמש ב-MaxYearRange
            // ואם כן צריך, ניתן לעדכן לשעות למשל:
            // MaxYearRange = (int)RiskTimeRange.TotalHours;
        };

        // התחלה
        CurrentTime = s_bl.Admin.GetClock();

        s_bl.Admin.AddClockObserver(clockObserver);
        s_bl.Admin.AddConfigObserver(configObserver);

        clockObserver();
        configObserver();
        CallList = s_bl.Call.GetFilteredAndSortedCallList();

    }

    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

    // תכונת טווח סיכון
    public TimeSpan RiskTimeRange
    {
        get { return (TimeSpan)GetValue(RiskTimeRangeProperty); }
        set { SetValue(RiskTimeRangeProperty, value); }
    }
    public static readonly DependencyProperty RiskTimeRangeProperty =
        DependencyProperty.Register("RiskTimeRange", typeof(TimeSpan), typeof(MainWindow));

    public string RiskTimeRangeText
    {
        get => RiskTimeRange.ToString();
        set
        {
            if (TimeSpan.TryParse(value, out var ts))
                RiskTimeRange = ts;
        }
    }

    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BL.BO.TimeUnit.Minute);
    }

    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BL.BO.TimeUnit.Hour);
    }

    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BL.BO.TimeUnit.Day);
    }

    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BL.BO.TimeUnit.Year);
    }
  
    //private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
    //{
    //    if (int.TryParse(MaxRange.ToString(), out int minutes))
    //    {
    //        try
    //        {
    //            RiskTimeRange = TimeSpan.FromMinutes(minutes);
    //            s_bl.Admin.SetRiskTimeRange(RiskTimeRange);

    //            MessageBox.Show($"Risk range updated to: {minutes} minutes.");
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show($"An error occurred while updating the risk range: {ex.Message}");
    //        }
    //    }
    //    else
    //    {
    //        MessageBox.Show("Please enter a valid numeric value for the risk range.");
    //    }
    //}
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

 
    public static readonly DependencyProperty MaxRangeProperty =
        DependencyProperty.Register("MaxRange", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // לא דרוש אם עושים הכל בבנאי
    }

    private void MainWindow_Closed(object sender, EventArgs e)
    {
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
    }

    // === אתחול בסיס הנתונים ===
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

                foreach (Window win in Application.Current.Windows)
                {
                    if (win != this)
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

    // === איפוס בסיס הנתונים ===
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
    public IEnumerable<BL.BO.CallInList> CallList
    {
        get { return (IEnumerable<BL.BO.CallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register(
            nameof(CallList),
            typeof(IEnumerable<BL.BO.CallInList>),
            typeof(MainWindow),
            new PropertyMetadata(null));

    private void btnShowVolunteerList_Click(object sender, RoutedEventArgs e)
    {
        var window = new Volunteer.VolunteerListWindow();
        window.Show(); 
    }


}
