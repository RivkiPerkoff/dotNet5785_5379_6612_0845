namespace DalTest;
using DalApi;
using DO;
using System.Text;

/// <summary>
/// A utility class to initialize the data source by creating and populating the Volunteers, Calls, and Assignments lists.
/// </summary>
public static class Initialization
{
    public static IDal? s_dal;

    //private static IVolunteer? s_dalVolunteer;
    //private static IAssignment? s_dalAssignment;
    //private static ICall? s_dalCall;
    //private static IConfig? s_dalConfig;

    private static readonly Random s_rand = new();

    /// <summary>
    /// Creates a list of volunteers with random data and adds them to the volunteer data source.
    /// </summary>
    private static void createVolunteer()
    {
        string[] names = { "Roni", "Maya", "Lior", "Noa", "Dani", "Tomer", "Liat", "Gal", "Yona", "Nir", "Omer", "Shira", "Erez", "Michal", "Hadar" };
        string[] emails = { "Roni@gmail.com", "Maya@gmail.com", "Lior@gmail.com", "Noa@gmail.com", "Dani@gmail.com", "Tomer@gmail.com", "Liat@gmail.com", "Gal@gmail.com", "Yona@gmail.com", "Nir@gmail.com", "Omer@gmail.com", "Shira@gmail.com", "Erez@gmail.com", "Michal@gmail.com", "Hadar@gmail.com" };
        string[] phones = { "050-321-7845", "052-987-1243", "054-123-9876", "053-549-4567", "055-789-1234", "050-987-6789", "054-112-7654", "053-785-6543", "052-312-7890", "055-654-3210", "053-789-5647", "050-125-1234", "052-456-8765", "054-654-4321", "055-123-4567" };
        string[] addresses = { "Nazareth", "Haifa", "Beersheba", "Eilat", "Rosh Ha'ayin", "Afula", "Kiryat Shmona", "Karmiel", "Dimona", "Tiberias", "Safed", "Kiryat Gat", "Ashkelon", "Lod", "Ramat Gan" };

        for (int i = 0; i < names.Length; i++)
        {
            int id = s_dal!.Config.CreateVolunteerId();
            string name = names[i];
            string email = emails[i];
            string phone = phones[i];
            string address = addresses[i];
            double maximumDistance = s_rand.NextDouble() * 50;
            string password = GenerateRandomPassword(12);

            s_dal!.Volunteer.Create(new Volunteer(
                id,
                name,
                email,
                phone,
                address,
                password
            ));
        }
    }

    /// <summary>
    /// Generates a random password that meets the security requirements:
    /// At least 8 characters, containing upper and lower case letters, a digit, and a special character.
    /// </summary>
    private static string GenerateRandomPassword(int length)
    {
        const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?/";

        // Ensuring at least one character from each required category
        StringBuilder password = new();
        password.Append(upperCase[s_rand.Next(upperCase.Length)]);
        password.Append(lowerCase[s_rand.Next(lowerCase.Length)]);
        password.Append(digits[s_rand.Next(digits.Length)]);
        password.Append(specialChars[s_rand.Next(specialChars.Length)]);

        // Filling the rest of the password with a mix of all characters
        string allChars = upperCase + lowerCase + digits + specialChars;
        for (int i = password.Length; i < length; i++)
        {
            password.Append(allChars[s_rand.Next(allChars.Length)]);
        }

        // Shuffling the characters to ensure randomness
        return new string(password.ToString().ToCharArray().OrderBy(_ => s_rand.Next()).ToArray());
    }
   

    /// <summary>
    /// Creates a list of calls with random data and adds them to the call data source.
    /// </summary>
    private static void createCall()
    {
        string[] descriptions = {
            "Ride to Ben Gurion Airport", "Ride to a family event", "Ride to work", "Return from the city", "Ride for shopping",
            "Ride to the train station", "Arrival at volunteer conference", "Return from school", "Ride to an evening class", "Ride to the hospital",
            "Ride to the airport", "Arrival at a group trip", "Ride to visit family", "Business meeting ride", "Ride to a birthday party"
        };

        string[] addresses = {
            "Agrat Moshe 9 Ramat Shlomo Jerusalem", "HaNasi 12 Tel Aviv", "Chaim Ozer 5 Haifa", "HaChoma 7 Be'er Sheva", "Herzl 22 Eilat",
            "HaGolan 14 Rishon LeZion", "HaAtzmaut 3 Netanya", "HaSivuv 10 Petah Tikva", "HaMeretz 5 Ra'anana", "Shderot Jerusalem 15 Hadera",
            "HaHadarim 4 Modi'in", "HaNamal 3 Ashdod", "Herzliya 19 Kiryat Shmona", "HaMaor 11 Safed", "HaHermon 7 Tiberias",
            "HaCarmel 8 Netanya", "Har Tzion 22 Jerusalem", "HaHored 3 Haifa", "Tel Aviv 10 Ibn Gvirol", "HaGalil 15 Petah Tikva"
        };
        const int totalCalls = 50;

        for (int i = 0; i < totalCalls; i++)
        {
            int callId = s_dal!.Config.CreateCallId();
            string description = descriptions[s_rand.Next(descriptions.Length)];
            string address = addresses[s_rand.Next(addresses.Length)];
            double latitude = s_rand.NextDouble() * (32.0 - 29.0) + 29.0;
            double longitude = s_rand.NextDouble() * (35.5 - 34.0) + 34.0;
            ///////////////////////////////////////////////////////////////////לשים לב לשעה
            DateTime start = new DateTime(s_dal.Config.Clock.Year - 1,s_dal.Config.Clock.Month,s_dal.Config.Clock.Day,s_dal.Config.Clock.Hour,0,0).AddHours(-5);
            int range = (s_dal.Config.Clock - start).Days;
            DateTime openingTime = start.AddDays(s_rand.Next(range));
            DateTime maxTimeToFinish = openingTime.AddDays(s_rand.Next((s_dal.Config.Clock - openingTime).Days)+1);

            s_dal!.Call.Create(new Call(
                callId,
                description,
                address,
                latitude,
                longitude,
                openingTime,
                maxTimeToFinish
            ));
        }
    }

    /// <summary>
    /// Creates assignments linking volunteers and calls with random data and adds them to the assignment data source.
    /// </summary>
    private static void createAssignment()
    {
        List<Call>? calls = s_dal!.Call.ReadAll().ToList();
        List<Volunteer>? volunteers = s_dal.Volunteer!.ReadAll().ToList();
        for (int i = 0; i < 50; i++)
        {
            if (calls[i].OpeningTime.HasValue && calls[i].MaxFinishTime.HasValue)
            {
                int id = s_dal!.Config.CreateAssignmentId();
                DateTime minTime = calls[i].OpeningTime!.Value;  // המרת OpeningTime ל-Value
                DateTime maxTime = calls[i].MaxFinishTime!.Value;  // המרת MaxFinishTime ל-Value

                int volunteerId = volunteers[s_rand.Next(volunteers.Count)].VolunteerId;

                TimeSpan difference = maxTime - minTime - TimeSpan.FromHours(2);
                DateTime randomTime = minTime.AddMinutes(s_rand.Next((int)difference.TotalMinutes));
                s_dal!.Assignment.Create(new Assignment(
                    id,
                    calls[i].IdCall,
                    volunteerId,
                    FinishCallType.TakenCareof,
                    randomTime,
                    (DateTime?)randomTime.AddHours(2)
                ));
            }
            else
            {
                Console.WriteLine($"Call at index {i} has null OpeningTime or MaxFinishTime, skipping...");
            }
        }
    }

    /// <summary>
    /// Initializes the DAL (Data Access Layer) by creating and adding volunteers, calls, and assignments to the system.
    /// </summary>
    /// <param name="dalVolunteer">The volunteer data access layer.</param>
    /// <param name="dalCall">The call data access layer.</param>
    /// <param name="dalAssignment">The assignment data access layer.</param>
    /// <param name="dalConfig">The configuration data access layer.</param>
    public static void DO()
    {
        //    s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!");
        s_dal = DalApi.Factory.Get;
        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();
        Console.WriteLine("Initializing Volunteers list ...");
        createVolunteer();
        Console.WriteLine("Initializing Calls list ...");
        createCall();
        Console.WriteLine("Initializing Assignment list ...");
        createAssignment();
    }
}
