namespace DalTest;
using Dal;
using DalApi;
using DO;
/// <summary>
/// The main class of the program.
/// Contains the main menu, sub-menus for manipulating entities like Volunteers, Calls, Assignments, and Configuration parameters.
/// </summary>
internal class Program
{
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); 
    private static ICall? s_dalCall = new CallImplementation(); 
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
    private enum ConfigSubmenu
    {
        Exit,
        AdvanceClockByMinute,
        AdvanceClockByHour,
        AdvanceClockByDay,
        AdvanceClockByMonth,
        AdvanceClockByYear,
        DisplayClock,
        ChangeClockOrRiskRange,
        DisplayConfigVar,
        Reset
    }
    private static void Main(string[] args)
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
                ConfigSubmenuu();
                break;
            case MainMenuOptions.InitializeData:
                Initialization.DO(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
                break;
            case MainMenuOptions.DisplayAllData:
                {
                    ReadAll("VolunteerSubmenu");
                    ReadAll("CallSubmenu");
                    ReadAll("AssignmentSubmenu");
                }
                break;
            case MainMenuOptions.ResetDatabase:
                s_dalConfig?.Reset(); //stage 1
                s_dalVolunteer?.DeleteAll(); //stage 1
                s_dalCall?.DeleteAll(); //stage 1
                s_dalAssignment?.DeleteAll(); //stage 1
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }


    private static void EntityMenu(MainMenuOptions choice)
    {
        //Console.WriteLine("Enter a number");
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
    private static void Create(string choice)
    {
        Console.WriteLine("Enter your details");
        Console.Write("Enter ID:");
        int yourId = int.Parse(Console.ReadLine()!);
        switch (choice)
        {
            case "VolunteerSubmenu":
                Volunteer Vol = CreateVolunteer(yourId);
                s_dalVolunteer?.Create(Vol);
                break;
            case "CallSubmenu":
                Call Call = CreateCall(yourId);
                s_dalCall?.Create(Call);
                break;
            case "AssignmentSubmenu":
                Assignment Ass = CreateAssignment(yourId);
                s_dalAssignment?.Create(Ass);
                break;

        }
    }
    private static Volunteer CreateVolunteer(int id)
    {
        Console.Write("Enter your name: ");
        string name = Console.ReadLine()!;

        Console.Write("Enter your email: ");
        string email = Console.ReadLine()!;

        Console.Write("Enter your phone: ");
        string phone = Console.ReadLine()!;

        Console.Write("Password (leave blank for null): ");
        string? password = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(password))
            password = null;

        Console.Write("Enter your address: ");
        string address = Console.ReadLine()!;

        Console.Write("Enter Maximum Distance: ");
        double maximumDistance = double.Parse(Console.ReadLine()!);

        Console.Write("Enter Latitude: ");
        double latitude = double.Parse(Console.ReadLine()!);

        Console.Write("Enter Longitude: ");
        double longitude = double.Parse(Console.ReadLine()!);

        Console.Write("Is Active? (true/false): ");
        bool isAvailable = bool.Parse(Console.ReadLine()!);

        Console.Write("Enter Role (1 for Manager, 2 for Volunteer): ");
        Role role = (Role)int.Parse(Console.ReadLine()!);

        Console.Write("Enter Distance Type (1 for Air, 2 for Walking, etc.): ");
        DistanceType distanceType = (DistanceType)int.Parse(Console.ReadLine()!);

        return new Volunteer(
            VolunteerId: id,
            Name: name,
            PhoneNumber: phone,
            EmailOfVolunteer: email,
            PasswordVolunteer: password,
            AddressVolunteer: address,
            VolunteerLatitude: latitude,
            VolunteerLongitude: longitude,
            IsAvailable: isAvailable,
            MaximumDistanceForReceivingCall: maximumDistance,
            Role: role,
            DistanceType: distanceType
        );
    }

    private static Call CreateCall(int id)
    {
        Console.Write("Enter Call Type (1 for Type1, 2 for Type2, etc.): ");
        TypeOfReading typeOfReading = (TypeOfReading)int.Parse(Console.ReadLine()!);

        Console.Write("Enter Description of the problem: ");
        string description = Console.ReadLine()!;

        Console.Write("Enter your address: ");
        string address = Console.ReadLine()!;

        Console.Write("Enter your Latitude: ");
        double latitude = double.Parse(Console.ReadLine()!);

        Console.Write("Enter your Longitude: ");
        double longitude = double.Parse(Console.ReadLine()!);

        Console.Write("Enter Opening Time (YYYY-MM-DD HH:MM): ");
        DateTime openingTime = DateTime.Parse(Console.ReadLine()!);

        Console.Write("Enter Max Time Finish Calling (YYYY-MM-DD HH:MM): ");
        DateTime maxClosing = DateTime.Parse(Console.ReadLine()!);

        return new Call(id, description, address, latitude, longitude, openingTime, maxClosing);
    }


    private static Assignment CreateAssignment(int id)
    {
        Console.Write("Enter Call ID: ");
        int CallId = int.Parse(Console.ReadLine()!);
        Console.Write("Enter Volunteer ID: ");
        int volunteerId = int.Parse(Console.ReadLine()!);
        Console.Write("Enter Type Of End Time : 1 for treated, 2 for Self Cancellation,3 for CancelingAnAdministrator,4 for CancellationHasExpired ");
        TypeOfEndTime typeOfEndTime = (TypeOfEndTime)int.Parse(Console.ReadLine()!);
        Console.Write("Enter Ending Time of Treatment ( YYYY-MM-DD HH:MM): ");
        DateTime EndTime = DateTime.Parse(Console.ReadLine()!);
        return new Assignment(id, CallId, volunteerId, typeOfEndTime, EndTime);
    }

    private static void Update(string choice)
    {
        Console.WriteLine("Enter your details");
        Console.Write("Enter ID: ");
        int yourId = int.Parse(Console.ReadLine()!);
        switch (choice)
        {
            case "VolunteerSubmenu":
                Volunteer Vol = CreateVolunteer(yourId);
                s_dalVolunteer?.Update(Vol);
                break;
            case "CallSubmenu":
                Call Call = CreateCall(yourId);
                s_dalCall?.Update(Call);
                break;
            case "AssignmentSubmenu":
                Assignment Ass = CreateAssignment(yourId);
                s_dalAssignment?.Update(Ass);
                break;
        }
    }
    private static void Read(string choice)
    {
        Console.WriteLine("Enter ID: ");
        int yourId = int.Parse(Console.ReadLine()!);
        switch (choice)
        {
            case "VolunteerSubmenu":
                s_dalVolunteer?.Read(yourId);
                break;
            case "CallSubmenu":
                s_dalCall?.Delete(yourId);
                break;
            case "AssignmentSubmenu":
                s_dalAssignment?.Delete(yourId);
                break;
        }
    }
    private static void ReadAll(string choice)
    {

        switch (choice)
        {
            case "VolunteerSubmenu":
                foreach (var item in s_dalVolunteer!.ReadAll())
                    Console.WriteLine(item);
                break;
            case "CallSubmenu":
                foreach (var item in s_dalCall!.ReadAll())
                    Console.WriteLine(item);
                break;
            case "AssignmentSubmenu":
                foreach (var item in s_dalAssignment!.ReadAll())
                    Console.WriteLine(item);
                break;
        }
    }
    private static void Delete(string choice)
    {
        Console.WriteLine("Enter ID: ");
        int yourId = int.Parse(Console.ReadLine()!);
        switch (choice)
        {
            case "VolunteerSubmenu":
                s_dalVolunteer?.Delete(yourId);
                break;
            case "CallSubmenu":
                s_dalCall?.Delete(yourId);
                break;
            case "AssignmentSubmenu":
                s_dalAssignment?.Delete(yourId);
                break;
        }
    }

    private static void DeleteAll(string choice)
    {
        switch (choice)
        {
            case "VolunteerSubmenu":
                s_dalVolunteer?.DeleteAll();
                break;
            case "CallSubmenu":
                s_dalCall?.DeleteAll();
                break;
            case "AssignmentSubmenu":
                s_dalAssignment?.DeleteAll();
                break;
        }

    }
    
    private static void ConfigSubmenuu()
    {
        Console.WriteLine("Config Menu:");
        foreach (ConfigSubmenu option in Enum.GetValues(typeof(ConfigSubmenu)))
        {
            Console.WriteLine($"{(int)option}. {option}");
        }
        Console.Write("Select an option: ");
        if (!Enum.TryParse(Console.ReadLine(), out ConfigSubmenu userInput)) throw new FormatException("Invalid choice");
        {
            while (userInput is not ConfigSubmenu.Exit)
            {
                switch (userInput)
                {
                    case ConfigSubmenu.AdvanceClockByMinute:

                        s_dalConfig!.Clock = s_dalConfig.Clock.AddMinutes(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByHour:
                        s_dalConfig!.Clock = s_dalConfig.Clock.AddHours(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByDay:
                        s_dalConfig!.Clock = s_dalConfig.Clock.AddDays(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByMonth:
                        s_dalConfig!.Clock = s_dalConfig.Clock.AddMonths(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByYear:
                        s_dalConfig!.Clock = s_dalConfig.Clock.AddYears(1);
                        break;
                    case ConfigSubmenu.DisplayClock:
                        Console.WriteLine(s_dalConfig!.Clock);
                        break;
                    case ConfigSubmenu.ChangeClockOrRiskRange:
                        Console.WriteLine($"RiskRange : {s_dalConfig!.RiskRange}");
                        break;
                    case ConfigSubmenu.DisplayConfigVar:
                        Console.Write("הזן ערך חדש עבור RiskRange (בפורמט שעות:דקות:שניות): ");
                        string riskRangeInput = Console.ReadLine()!;
                        if (!TimeSpan.TryParse(riskRangeInput, out TimeSpan newRiskRange)) throw new FormatException("Invalid choice");
                        {
                            s_dalConfig!.RiskRange=newRiskRange;
                            Console.WriteLine($"RiskRange update to: {s_dalConfig.RiskRange}");
                        }
                        break;

                    case ConfigSubmenu.Reset:
                        s_dalConfig?.Reset();
                        break;


                }

            }
        }
    }

}