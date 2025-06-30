using System.Windows;

namespace PL
{
    public partial class ChooseScreenWindow : Window
    {
        public bool GoToManager { get; private set; } = false;
        public int? VolunteerId { get; private set; } = null;

        public ChooseScreenWindow()
        {
            InitializeComponent();
        }

        private void ManagerButton_Click(object sender, RoutedEventArgs e)
        {
            GoToManager = true;
            DialogResult = true;
        }

        //private void VolunteerButton_Click(object sender, RoutedEventArgs e)
        //{
        //    GoToManager = false;
        //    DialogResult = true;
        //}
        private void VolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            GoToManager = false;
            DialogResult = true;
        }


    }
}
