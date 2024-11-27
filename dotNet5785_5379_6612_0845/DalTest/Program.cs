using Dal;
using DalApi;
using DO;
using DalList;

namespace DalTest;

internal class Program
{
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); // stage 1
    private static ICall? s_dalCall = new CallImplementation(); // stage 1
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); // stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation();

    private enum MainMenuOptions
    {
        Exit,
        VolunteerSubMenu,
        CallSubMenu,
        AssignmentSubMenu,
        ConfigurationSubMenu,
        InitializeData,
        DisplayAllData,
        ResetDatabase
    }
    public enum SubMenu
    {
        Exit,
        Create,
        Read,
        ReadAll,
        UpDate,
        Delete,
        DeleteAll
    }

    static void Main(string[] args)
    {
        try
        {
            while (true)
            {
                ShowMainMenu();
                Console.WriteLine("Please choose an option: ");
                if (!Enum.TryParse(Console.ReadLine(), out MainMenuOptions option) || !Enum.IsDefined(option))
                {
                    Console.WriteLine("Invalid option. Please try again.");
                    continue;
                }
                HandleMainMenuOption(option);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occurred: {ex.Message}");
        }
    }

    private static void ShowMainMenu()
    {
        Console.WriteLine("\n=== Main Menu ===");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Volunteer Sub-Menu");
        Console.WriteLine("2. Call Sub-Menu");
        Console.WriteLine("3. Assignment Sub-Menu");
        Console.WriteLine("4. Configuration Sub-Menu");
        Console.WriteLine("5. Initialize Data");
        Console.WriteLine("6. Display All Data");
        Console.WriteLine("7. Reset Database");
        Console.WriteLine("=================");
    }

    private static void HandleMainMenuOption(MainMenuOptions option)
    {
        switch (option)
        {
            case MainMenuOptions.Exit:
                Console.WriteLine("Exiting the program...");
                Environment.Exit(0);
                break;
            case MainMenuOptions.VolunteerSubMenu:
            case MainMenuOptions.CallSubMenu:
            case MainMenuOptions.AssignmentSubMenu:
                EntityMenu(option);
                break;
            case MainMenuOptions.ConfigurationSubMenu:
                ShowConfigurationSubMenu();
                break;
            case MainMenuOptions.InitializeData:
                InitializeData();
                break;
            case MainMenuOptions.DisplayAllData:
                DisplayAllData();
                break;
            case MainMenuOptions.ResetDatabase:
                ResetDatabase();
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }


    private static void EntityMenu(MainMenuOptions choice)
    {
        Console.WriteLine("Enter a number");
        foreach (SubMenu option in Enum.GetValues(typeof(SubMenu)))
        {
            Console.WriteLine($"{(int)option}. {option}");
        }
        if (!Enum.TryParse(Console.ReadLine(), out SubMenu subChoice)) throw new FormatException("Invalid choice");
        while (subChoice != 0)
        {
            switch (subChoice)
            {
                case SubMenu.Create:
                    Create(choice.ToString()); // כאן תוכל להמיר ל-string לפי הצורך
                    break;
                case SubMenu.Read:
                    Console.WriteLine("Enter Your ID");
                    Read(choice.ToString());
                    break;
                case SubMenu.ReadAll:
                    ReadAll(choice.ToString());
                    break;
                case SubMenu.Delete:
                    Delete(choice.ToString());
                    break;
                case SubMenu.DeleteAll:
                    DeleteAll(choice.ToString());
                    break;
                case SubMenu.UpDate:
                    Update(choice.ToString());
                    break;
                case SubMenu.Exit:
                    return;
                default:
                    Console.WriteLine("Your choice is not valid, please enter again");
                    break;
            }
            Console.WriteLine("Enter a number");
            Enum.TryParse(Console.ReadLine(), out subChoice);
        }
    }


    private static void ShowConfigurationSubMenu()
    {
        Console.WriteLine("\n=== Configuration Sub-Menu ===");
        // Add options and logic for configuration-specific operations
    }

    private static void InitializeData()
    {
        Console.WriteLine("Initializing data...");
        Initialization.DO(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
    }

    private static void DisplayAllData()
    {
        Console.WriteLine("\n=== Display All Data ===");
        Console.WriteLine("Volunteers:");
        var volunteers = s_dalVolunteer!.ReadAll();
        foreach (var volunteer in volunteers)
        {
            Console.WriteLine(volunteer);
        }

        Console.WriteLine("\nCalls:");
        var calls = s_dalCall!.ReadAll();
        foreach (var call in calls)
        {
            Console.WriteLine(call);
        }

        Console.WriteLine("\nAssignments:");
        var assignments = s_dalAssignment!.ReadAll();
        foreach (var assignment in assignments)
        {
            Console.WriteLine(assignment);
        }
    }

    private static void ResetDatabase()
    {
        Console.WriteLine("Resetting database...");
        s_dalConfig!.Reset();
        s_dalVolunteer!.DeleteAll();
        s_dalCall!.DeleteAll();
        s_dalAssignment!.DeleteAll();
        Console.WriteLine("Database reset successfully.");
    }
}


