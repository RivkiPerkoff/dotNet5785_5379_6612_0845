namespace DalTest;
using DalApi;
using DO;
using System.Net.Mail;
using System.Numerics;
using System.Xml.Linq;

//using System.Xml.Linq;

public static class Initialization
{
    private static IVolunteer? s_dalVolunteer;
    private static IConfig? s_dalConfig;
    //private static IAssignment? s_dalAssignment;
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

            s_dalVolunteer!.Create(new Volunteer
            {
                VolunteerId = id,
                Name = name,
                EmailOfVolunteer = email,
                PhoneNumber = phone,
                AddressVolunteer = address,
                MaximumDistanceForReceivingCall = maximumDistance
            });
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

        // מאגר כתובות
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

        // מספר הקריאות הכולל
        const int totalCalls = 50;
        // לפחות 15 קריאות שלא הוקצו
        const int unassignedCalls = 15;
        // לפחות 5 קריאות שפג תוקפן
        const int expiredCalls = 5;

        for (int i = 0; i < totalCalls; i++)
        {
            // יצירת מזהה קריאה ייחודי מישות הקונפיגורציה
            int callId = s_dalConfig!.Create();

            // בחירת תיאור וכתובת רנדומליים
            string description = descriptions[s_rand.Next(descriptions.Length)];
            string address = addresses[s_rand.Next(addresses.Length)];

            // יצירת מיקום רנדומלי
            double latitude = s_rand.NextDouble() * (32.0 - 29.0) + 29.0; // בין קווי רוחב של ישראל
            double longitude = s_rand.NextDouble() * (35.5 - 34.0) + 34.0; // בין קווי אורך של ישראל

            // יצירת זמן פתיחה רנדומלי
            DateTime openingTime = DateTime.Now.AddMinutes(-s_rand.Next(0, 60 * 24 * 7)); // עד שבוע אחורה

            // קביעת תוקף הקריאה
            bool isExpired = i < expiredCalls;
            if (isExpired)
            {
                openingTime = openingTime.AddDays(-s_rand.Next(1, 5)); // קריאות שפג תוקפן (לפני כמה ימים)
            }

            // האם הקריאה הוקצתה
            bool isAssigned = i >= unassignedCalls;

            // שמירת הקריאה במאגר
            s_dalCall!.Create(new Call(
                callId,
                description,
                address,
                latitude,
                longitude,
                openingTime
            ));
        }
    }


    private static void createConfig()
    {
        // Implement the method for creating config here.
    }
}
