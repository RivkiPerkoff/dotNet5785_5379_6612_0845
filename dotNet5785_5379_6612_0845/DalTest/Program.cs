namespace DalTest;
using Dal;
using DalApi;
using DO;
using System.Diagnostics;

/// <summary>
/// The main class of the program.
/// Contains the main menu, sub-menus for manipulating entities like Volunteers, Calls, Assignments, and Configuration parameters.
/// </summary>
internal class Program
{
    //    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); 
    //    private static ICall? s_dalCall = new CallImplementation(); 
    //    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); // stage 1
    //    private static IConfig? s_dalConfig = new ConfigImplementation();
    static readonly IDal s_dal = new DalList();
    
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
        //try
        //{
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
                    Initialization.DO(s_dal);
                    break;
                case MainMenuOptions.DisplayAllData:
                    {
                        ReadAll("VolunteerSubMenu");
                        ReadAll("CallSubMenu");
                        ReadAll("AssignmentSubMenu");
                    }
                    break;
                case MainMenuOptions.ResetDatabase:
                    s_dal.Config!.Reset(); //stage 1
                    s_dal.Volunteer!.DeleteAll(); //stage 1
                    s_dal.Call!.DeleteAll(); //stage 1
                    s_dal.Assignment!.DeleteAll(); //stage 1
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        //}
        //catch (Exception ex) {
        //    Console.WriteLine($"An exception occurred: {ex.Message}");
        //}
    }


    private static void EntityMenu(MainMenuOptions choice)
    {
        try
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
                    Create(choice.ToString());
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
        catch (Exception ex)
        {
            Console.WriteLine($"An exception occurred: {ex.Message}");
        }
    }
    private static void Create(string choice)
    {
        Console.WriteLine("Enter your details");
        Console.Write("Enter ID:");
        int yourId = int.Parse(Console.ReadLine()!);
        switch (choice)
        {
            case "VolunteerSubMenu":
                Volunteer Vol = CreateVolunteer(yourId);
                s_dal.Volunteer?.Create(Vol);
                break;
            case "CallSubMenu":
                Call Call = CreateCall(yourId);
                s_dal.Call?.Create(Call);
                break;
            case "AssignmentSubMenu":
                Assignment Ass = CreateAssignment(yourId);
                s_dal.Assignment?.Create(Ass);
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
            case "VolunteerSubMenu":
                Volunteer Vol = CreateVolunteer(yourId);
                s_dal.Volunteer?.Update(Vol);
                break;
            case "CallSubMenu":
                Call Call = CreateCall(yourId);
                s_dal.Call?.Update(Call);
                break;
            case "AssignmentSubMenu":
                Assignment Ass = CreateAssignment(yourId);
                s_dal.Assignment?.Update(Ass);
                break;
        }
    }
    private static void Read(string choice)
    {
        Console.WriteLine("Enter ID: ");
        int yourId = int.Parse(Console.ReadLine()!);
        switch (choice)
        {
            case "VolunteerSubMenu":
                Console.WriteLine(s_dal.Volunteer?.Read(yourId));
                break;
            case "CallSubMenu":
                Console.WriteLine(s_dal.Call?.Read(yourId));
                break;
            case "AssignmentSubMenu":
                Console.WriteLine(s_dal.Assignment?.Read(yourId));
                break;
        }
    }
    private static void ReadAll(string choice)
    {

        switch (choice)
        {
            case "VolunteerSubMenu":
                foreach (var item in s_dal.Volunteer!.ReadAll())
                    Console.WriteLine(item);
                break;
            case "CallSubMenu":
                foreach (var item in s_dal.Call!.ReadAll())
                    Console.WriteLine(item);
                break;
            case "AssignmentSubMenu":
                foreach (var item in s_dal.Assignment!.ReadAll())
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
            case "VolunteerSubMenu":
                s_dal.Volunteer?.Delete(yourId);
                break;
            case "CallSubMenu":
                s_dal.Call?.Delete(yourId);
                break;
            case "AssignmentSubMenu":
                s_dal.Assignment?.Delete(yourId);
                break;
        }
    }

    private static void DeleteAll(string choice)
    {
        switch (choice)
        {
            case "VolunteerSubMenu":
                s_dal.Volunteer?.DeleteAll();
                break;
            case "CallSubMenu":
                s_dal.Call?.DeleteAll();
                break;
            case "AssignmentSubMenu":
                s_dal.Assignment?.DeleteAll();
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

                        s_dal.Config!.Clock = s_dal.Config.Clock.AddMinutes(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByHour:
                        s_dal.Config!.Clock = s_dal.Config.Clock.AddHours(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByDay:
                        s_dal.Config!.Clock = s_dal.Config.Clock.AddDays(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByMonth:
                        s_dal.Config!.Clock = s_dal.Config.Clock.AddMonths(1);
                        break;
                    case ConfigSubmenu.AdvanceClockByYear:
                        s_dal.Config!.Clock = s_dal.Config.Clock.AddYears(1);
                        break;
                    case ConfigSubmenu.DisplayClock:
                        Console.WriteLine(s_dal.Config!.Clock);
                        break;
                    case ConfigSubmenu.ChangeClockOrRiskRange:
                        Console.WriteLine($"RiskRange : {s_dal.Config!.RiskRange}");
                        break;
                    case ConfigSubmenu.DisplayConfigVar:
                        Console.Write("Enter a new value for RiskRange (In format hours:minutes:seconds ): ");
                        string riskRangeInput = Console.ReadLine()!;
                        if (!TimeSpan.TryParse(riskRangeInput, out TimeSpan newRiskRange)) throw new FormatException("Invalid choice");
                        {
                            s_dal.Config!.RiskRange=newRiskRange;
                            Console.WriteLine($"RiskRange update to: {s_dal.Config.RiskRange}");
                        }
                        break;

                    case ConfigSubmenu.Reset:
                        s_dal.Config!.Reset();
                        break;


                }

            }
        }
    }

}