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

            // רישום מתודה שתופעל כאשר השעון משתנה
            s_bl.Admin.AddClockObserver(UpdateClockDisplay);

            // הצגת הזמן הנוכחי הראשוני
            CurrentTime = s_bl.Admin.GetClock();
        }
        private void UpdateClockDisplay()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
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

        //private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        //{
        //    s_bl.Admin.AddClockObserver(BL.BO.TimeUnit.Minute);
        //}
        //private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        //{
        //    s_bl.Admin.AddClockObserver(BL.BO.TimeUnit.Hour);
        //}
        //private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        //{
        //    s_bl.Admin.AddClockObserver(BL.BO.TimeUnit.Day);
        //}
        //private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        //{
        //    s_bl.Admin.AddClockObserver(BL.BO.TimeUnit.Year);
        //}
    }
}