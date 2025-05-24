using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using BL.BO;

namespace BO.Volunteer
{
    public partial class VolunteerWindow : Window, INotifyPropertyChanged
    {
        private Volunteer _volunteer;
        public Volunteer Volunteer
        {
            get => _volunteer;
            set
            {
                _volunteer = value;
                OnPropertyChanged(nameof(Volunteer));
            }
        }

        private string _buttonText = "Add";
        public string ButtonText
        {
            get => _buttonText;
            set { _buttonText = value; OnPropertyChanged(nameof(ButtonText)); }
        }

        // דוגמאות לאוספים של enum להצגה בקומבו
        public ObservableCollection<Role> Roles { get; set; }
        public ObservableCollection<DistanceType> DistanceTypes { get; set; }

        public VolunteerWindow(int id = 0)
        {
            // יוזם את האובייקט לפני InitializeComponent
            Volunteer = new Volunteer();
            Roles = new ObservableCollection<Role>((Role[])Enum.GetValues(typeof(Role)));
            DistanceTypes = new ObservableCollection<DistanceType>((DistanceType[])Enum.GetValues(typeof(DistanceType)));

            ButtonText = id == 0 ? "Add" : "Update";

            InitializeComponent();

            this.DataContext = this;

            if (id != 0)
            {
                // כאן יש לאכלס את ה-Volunteer מנתונים קיימים (לדוגמה BL)
                LoadVolunteer(id);
            }
        }

        private void LoadVolunteer(int id)
        {
            // לדוגמה - כאן תטען את ה-Volunteer לפי id מתוך BL
            // Volunteer = yourBL.GetVolunteerById(id);

            // להדגמה, נניח ממלאים ידנית:
            Volunteer = new Volunteer
            {
                VolunteerId = id,
                Name = "Moshe",
                PhoneNumber = "050-1234567",
                EmailOfVolunteer = "moshe@example.com",
                PasswordVolunteer = "secret",
                AddressVolunteer = "Rehov Hagefen 12",
                VolunteerLatitude = 32.0853,
                VolunteerLongitude = 34.7818,
                IsAvailable = true,
                MaximumDistanceForReceivingCall = 10,
                Role = Role.Admin,
                DistanceType = DistanceType.Kilometers
            };
            OnPropertyChanged(nameof(Volunteer));
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonText == "Add")
            {
                // הוספת וולונטר חדש
                MessageBox.Show("Adding new Volunteer...");
                // כאן תקרא לשכבת BL להוספה
            }
            else
            {
                // עדכון וולונטר קיים
                MessageBox.Show("Updating existing Volunteer...");
                // כאן תקרא לשכבת BL לעדכון
            }
        }

        // לעדכון סיסמה ב-Volunteer.PasswordVolunteer
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as System.Windows.Controls.PasswordBox;
            if (passwordBox != null)
            {
                Volunteer.PasswordVolunteer = passwordBox.Password;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
