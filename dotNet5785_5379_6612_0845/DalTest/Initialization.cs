namespace DalTest;
using DalApi;
using DO;
using System.Net.Mail;
using System.Numerics;
using System.Xml.Linq;
public static class Initialization
{
    private static IVolunteer? s_dalVolunteer;
    private static IConfig? s_dalConfig;
    private static IAssignment? s_dalAssignment;
    private static ICall? s_dalCall;

    private static readonly Random s_rand = new();

    private static void createVolunteer()
    {
        string[] names = {
            "Roni", "Maya", "Lior", "Noa", "Dani",
            "Tomer", "Liat", "Gal", "Yona", "Nir",
            "Omer", "Shira", "Erez", "Michal", "Hadar"
        };

        string[] emails = {
            "Roni@gmail.com", "Maya@gmail.com", "Lior@gmail.com", "Noa@gmail.com", "Dani@gmail.com",
            "Tomer@gmail.com", "Liat@gmail.com", "Gal@gmail.com", "Yona@gmail.com", "Nir@gmail.com",
            "Omer@gmail.com", "Shira@gmail.com", "Erez@gmail.com", "Michal@gmail.com", "Hadar@gmail.com"
        };

        string[] phones = {
            "050-321-7845", "052-987-1243", "054-123-9876", "053-549-4567", "055-789-1234",
            "050-987-6789", "054-112-7654", "053-785-6543", "052-312-7890", "055-654-3210",
            "053-789-5647", "050-125-1234", "052-456-8765", "054-654-4321", "055-123-4567"
        };

        string[] addresses = {
            "Nazareth", "Haifa", "Beersheba", "Eilat", "Rosh Ha'ayin",
            "Afula", "Kiryat Shmona", "Karmiel", "Dimona", "Tiberias",
            "Safed", "Kiryat Gat", "Ashkelon", "Lod", "Ramat Gan"
        };

        for (int i = 0; i < names.Length; i++)
        {
            int id;
            do
            {
                id = s_rand.Next(1000, 9999);
            }
            while (s_dalVolunteer!.Read(id) != null); // Make sure the ID is unique

            string name = names[i];
            string email = emails[i];
            string phone = phones[i];
            string address = addresses[i];

            double maximumDistance = s_rand.NextDouble() * 50;

            s_dalVolunteer!.Create(new Volunteer(
             id,
             name,
             email,
             phone,
             address
             ));
        }
    }
    private static void createCall()
    {
        // מאגר תיאורים לקריאות
        string[] descriptions = {
        "Ride to Ben Gurion Airport", "Ride to a family event", "Ride to work", "Return from the city", "Ride for shopping",
        "Ride to the train station", "Arrival at volunteer conference", "Return from school", "Ride to an evening class", "Ride to the hospital",
        "Ride to the airport", "Arrival at a group trip", "Ride to visit family", "Business meeting ride",
        "Ride to a birthday party"
    };

        string[] addresses = {
    "Agrat Moshe 9 Ramat Shlomo Jerusalem",
    "HaNasi 12 Tel Aviv",
    "Chaim Ozer 5 Haifa",
    "HaChoma 7 Be'er Sheva",
    "Herzl 22 Eilat",
    "HaGolan 14 Rishon LeZion",
    "HaAtzmaut 3 Netanya",
    "HaSivuv 10 Petah Tikva",
    "HaMeretz 5 Ra'anana",
    "Shderot Jerusalem 15 Hadera",
    "HaHadarim 4 Modi'in",
    "HaNamal 3 Ashdod",
    "Herzliya 19 Kiryat Shmona",
    "HaMaor 11 Safed",
    "HaHermon 7 Tiberias",
    "HaCarmel 8 Netanya",
    "Har Tzion 22 Jerusalem",
    "HaHored 3 Haifa",
    "Tel Aviv 10 Ibn Gvirol",
    "HaGalil 15 Petah Tikva"
    };
        const int totalCalls = 50;

        for (int i = 0; i < totalCalls; i++)
        {
            int callId = s_dalConfig!.Create();
            string description = descriptions[s_rand.Next(descriptions.Length)];
            string address = addresses[s_rand.Next(addresses.Length)];
            double latitude = s_rand.NextDouble() * (32.0 - 29.0) + 29.0;
            double longitude = s_rand.NextDouble() * (35.5 - 34.0) + 34.0;


            DateTime start = new DateTime(s_dalConfig.Clock.Year, s_dalConfig.Clock.Month, s_dalConfig.Clock.Day, s_dalConfig.Clock.Hour - 5, 0, 0);
            // חישוב הטווח של הימים בין השעון הנוכחי לבין תאריך ההתחלה
            int range = (s_dalConfig.Clock - start).Days;
            // יצירת תאריך אקראי בטווח הזה
            DateTime openingTime = start.AddDays(s_rand.Next(range));
            DateTime MaxTimeToFinish = openingTime.AddDays(s_rand.Next((s_dalConfig.Clock - openingTime).Days));

            //CallTypes callType = CallTypes
            s_dalCall!.Create(new Call(
                callId,
                description,
                address,
                latitude,
                longitude,
                openingTime,
                MaxTimeToFinish
            ));
        }
    }

    //private static void createAssignment()
    //{
    //    List<Call>? calls = s_dalCall!.ReadAll();
    //    for (int i = 0; i < 50; i++)
    //    {
    //        DateTime minTime = calls[i].OpeningTime;
    //        DateTime maxTime = (DateTime)calls[i].MaxFinishTime!;
    //        TimeSpan difference = maxTime - minTime - TimeSpan.FromHours(2);
    //        DateTime randomTime = minTime.AddMinutes(s_rand.Next((int)difference.TotalMinutes));

    //        s_dalAssignment!.Create(new Assignment(
    //            randomTime,
    //            randomTime.AddHours(2),
    //         (FinishCallType)s_rand.Next(Enum.GetValues(typeof(FinishCallType)).Length - 1)));
    //    }
    //}

    private static void createAssignment()
    {
        List<Call>? calls = s_dalCall!.ReadAll();
        for (int i = 0; i < 50; i++)
        {
            // בדיקה אם OpeningTime לא null
            if (calls[i].OpeningTime.HasValue && calls[i].MaxFinishTime.HasValue)
            {
                DateTime minTime = calls[i].OpeningTime.Value;  // המרת OpeningTime ל-Value
                DateTime maxTime = calls[i].MaxFinishTime.Value;  // המרת MaxFinishTime ל-Value

                TimeSpan difference = maxTime - minTime - TimeSpan.FromHours(2);
                DateTime randomTime = minTime.AddMinutes(s_rand.Next((int)difference.TotalMinutes));
                s_dalAssignment!.Create(new Assignment(
                    randomTime,                                       
                    TypeOfEndTime.treated,
                    randomTime.AddHours(2)
                ));
            }
            else
            {
                Console.WriteLine($"Call at index {i} has null OpeningTime or MaxFinishTime, skipping...");
            }
        }
    }



    public static void DO(IVolunteer? dalVolunteer, ICall? dalCall, IAssignment? dalAssignment, IConfig? dalConfig)
    {
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");

        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset();
        s_dalVolunteer.DeleteAll();
        s_dalCall.DeleteAll();
        s_dalAssignment.DeleteAll();

        Console.WriteLine("Initializing Volunteers list ...");
        createVolunteer();
        Console.WriteLine("Initializing Calls list ...");
        createCall();
        Console.WriteLine("Initializing Assignment list ...");
        createAssignment();
    }
}
