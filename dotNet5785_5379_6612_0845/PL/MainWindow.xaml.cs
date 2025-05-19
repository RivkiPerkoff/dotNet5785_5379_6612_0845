using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BL.BIApi;

namespace PL
{
    public partial class MainWindow : Window
    {
        private static readonly IBL s_bl = BlApi.Factory.Get();

        // משתנים לשמירת המשקיפים
        private readonly Action clockObserver;
        private readonly Action configObserver;

        public MainWindow()
        {
            InitializeComponent();
            CurrentTime = s_bl.Admin.GetClock();

            // הצגת הזמן הנוכחי הראשוני
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);

            // לקרוא אותן בפעם הראשונה כדי לקבל את הערכים ההתחלתיים
            clockObserver();
            configObserver();
        }
 
        //private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{

        //}
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

        private void UpdateClockDisplay()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void UpdateConfigDisplay()
        {
            RiskTimeRange = s_bl.Admin.GetRiskTimeRange();
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
        private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TimeSpan range = TimeSpan.FromDays(MaxYearRange * 365);
                s_bl.Admin.SetRiskTimeRange(range);

                MessageBox.Show("משתנה התצורה עודכן בהצלחה!", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"אירעה שגיאה בעת עדכון משתנה התצורה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public int MaxYearRange
        {
            get { return (int)GetValue(MaxYearRangeProperty); }
            set { SetValue(MaxYearRangeProperty, value); }
        }

        public static readonly DependencyProperty MaxYearRangeProperty =
            DependencyProperty.Register("MaxYearRange", typeof(int), typeof(MainWindow), new PropertyMetadata(0));


        private void clockObserver()
        {
            // מקבל את הזמן מה- BL ומעדכן את ה- CurrentTime
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void configObserver()
        {
            // מקבל את ערך משתנה התצורה ומעדכן אותו
            MaxYearRange = s_bl.Admin.GetRiskTimeRange().Days / 365;
        }

        private void clockObserver(object sender, EventArgs e)
        {
            s_bl.Admin.GetClock();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // לא דרוש אם עושים הכל בבנאי
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ללא מימוש כרגע
        }

        // === כפתור פתיחת תצוגת קורסים ===
        private void btnCourses_Click(object sender, RoutedEventArgs e)
        {
            new CourseListWindow().Show(); // חשוב שהחלון הזה קיים בפרויקט
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

                    s_bl.Admin.InitializeDB();
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

                    s_bl.Admin.ResetDB();
                    MessageBox.Show("בסיס הנתונים אופס בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }
    }
}
