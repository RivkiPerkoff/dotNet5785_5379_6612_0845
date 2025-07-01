using System.Windows;

namespace PL.VolunteerPersonal;

public partial class VolunteerCallHistoryWindow : Window
{
    public VolunteerCallHistoryWindow(int volunteerId)
    {
        InitializeComponent();
        DataContext = new VolunteerCallHistoryViewModel(volunteerId);
    }
}
