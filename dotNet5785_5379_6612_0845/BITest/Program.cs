
namespace BITest;
internal class Program
{
    static readonly IBL s_bl = BlApi.Factory.Get();

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Volunteer Management");
            Console.WriteLine("2. Call Management");
            Console.WriteLine("3. Admin Management");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

            switch (choice)
            {
                case 1:
                    VolunteerMenu();
                    break;
                case 2:
                    CallMenu();
                    break;
                case 3:
                    AdminMenu();
                    break;
                case 0:
                    return;
            }
        }
    }

    static void VolunteerMenu()
    {
        while (true)
        {
            Console.WriteLine("Volunteer Management:");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. List Volunteers");
            Console.WriteLine("3. Get Volunteer Details");
            Console.WriteLine("4. Update Volunteer");
            Console.WriteLine("5. Delete Volunteer");
            Console.WriteLine("6. Add Volunteer");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

            try
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("Username: ");
                        string username = Console.ReadLine();
                        Console.Write("Password: ");
                        string password = Console.ReadLine();
                        Console.WriteLine(s_bl.Volunteer.Login(username, password));
                        break;
                    case 2:
                        foreach (var v in s_bl.Volunteer.GetVolunteers(null, null))
                            Console.WriteLine(v);
                        break;
                    case 3:
                        Console.Write("Enter Volunteer ID: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                            Console.WriteLine(s_bl.Volunteer.GetVolunteerDetails(id));
                        break;
                    case 0:
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    static void CallMenu()
    {
        while (true)
        {
            Console.WriteLine("Call Management:");
            Console.WriteLine("1. List Calls");
            Console.WriteLine("2. Delete Call");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

            try
            {
                switch (choice)
                {
                    case 1:
                        foreach (var c in s_bl.Call.GetFilteredAndSortedCallList(null, null, null))
                            Console.WriteLine(c);
                        break;
                    case 2:
                        Console.Write("Enter Call ID: ");
                        if (int.TryParse(Console.ReadLine(), out int callId))
                            s_bl.Call.DeleteCall(callId);
                        break;
                    case 0:
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    static void AdminMenu()
    {
        while (true)
        {
            Console.WriteLine("Admin Management:");
            Console.WriteLine("1. Reset Database");
            Console.WriteLine("2. Initialize Database");
            Console.WriteLine("3. Get Clock");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

            try
            {
                switch (choice)
                {
                    case 1:
                        s_bl.Admin.ResetDatabase();
                        Console.WriteLine("Database reset successfully.");
                        break;
                    case 2:
                        s_bl.Admin.InitializeDatabase();
                        Console.WriteLine("Database initialized successfully.");
                        break;
                    case 3:
                        Console.WriteLine(s_bl.Admin.GetClock());
                        break;
                    case 0:
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
}
}
