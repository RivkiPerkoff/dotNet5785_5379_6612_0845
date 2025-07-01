﻿using BL.BIApi;
using PL.Volunteer;
using PL.VolunteerPersonal;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace PL
{
    public partial class LoginWindow : Window
    {
        private static readonly IBL s_bl = BlApi.Factory.Get();

        public string Username
        {
            get => (string)GetValue(UsernameProperty);
            set => SetValue(UsernameProperty, value);
        }

        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(LoginWindow));

        public string Password { get; set; } = string.Empty;

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string role = s_bl.Volunteer.Login(Username, Password);

                if (role == "Manager")
                {
                    bool managerWindowOpen = Application.Current.Windows
                        .OfType<MainWindow>()
                        .Any();

                    if (managerWindowOpen)
                    {
                        MessageBox.Show("כבר יש מנהל מחובר למערכת.", "אזהרה", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!int.TryParse(Username, out int managerAsVolunteerId))
                    {
                        MessageBox.Show("מזהה משתמש אינו תקין.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var chooseWindow = new ChooseScreenWindow
                    {
                        Owner = this
                    };

                    if (chooseWindow.ShowDialog() == true)
                    {
                        if (chooseWindow.GoToManager)
                        {
                            var mainWindow = new MainWindow(BL.BO.Role.Manager, managerAsVolunteerId);
                            mainWindow.Show();
                        }
                        else
                        {
                            VolunteerPersonalWindow volunteerWindow = new VolunteerPersonalWindow(managerAsVolunteerId);
                            volunteerWindow.Show();
                        }
                    }
                }
                else
                {
                    if (!int.TryParse(Username, out int volunteerId))
                    {
                        MessageBox.Show("פורמט מזהה שגוי.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    VolunteerPersonalWindow volunteerWindow = new VolunteerPersonalWindow(volunteerId);
                    volunteerWindow.Show();
                }
            }
            catch
            {
                MessageBox.Show("פרטי התחברות שגויים. אנא בדוק שוב.", "שגיאת התחברות", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                Password = passwordBox.Password;
            }
        }
    }
}

