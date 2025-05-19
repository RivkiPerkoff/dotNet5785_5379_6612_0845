using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BL.BIApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly BL.BIApi.IBL s_bl = BlApi.Factory.Get();

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



    }
}