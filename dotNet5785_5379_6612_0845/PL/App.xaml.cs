using System.Configuration;
using System.Data;
using System.Windows;


/// <summary>
/// Interaction logic for App.xaml
/// </summary>
namespace PL;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var loginWindow = new LoginWindow();
        loginWindow.Show();
    }
}

