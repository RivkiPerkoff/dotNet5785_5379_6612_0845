using System.Windows;
using System.Windows.Controls;
using BL.BO; // וודא שיש reference לפרויקט שבו נמצאת המחלקה Volunteer

namespace BO.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        private BL.BO.Volunteer _volunteer;

        public VolunteerWindow()
        {
            InitializeComponent();
            _volunteer = new BL.BO.Volunteer(); // יצירת מופע חדש או קבלת מופע קיים
            this.DataContext = _volunteer;
        }

        // טיפול בעדכון סיסמה כי PasswordBox לא תומך ב-Binding ל-Password
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                _volunteer.PasswordVolunteer = passwordBox.Password;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // כאן אפשר לכתוב קוד לשמירת הנתונים או עדכון
            MessageBox.Show($"Volunteer saved:\nName: {_volunteer.Name}\nPhone: {_volunteer.PhoneNumber}");
        }
    }
}
