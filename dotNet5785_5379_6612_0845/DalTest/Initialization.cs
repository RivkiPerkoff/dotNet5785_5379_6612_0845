namespace DalTest;
using DalApi;
using DO;
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
                id = s_rand.Next(100000000, 999999999);
            }
            while (s_dalVolunteer!.Read(id) != null); // Make sure the ID is unique

            string name = names[i];
            string email = emails[i];
            string phone = phones[i];
            string address = addresses[i];

            s_dalVolunteer!.Create(new Volunteer
            {
                VolunteerId = id,
                Name = name,
                Email = email,
                Phone = phone,
                Address = address
            });
        }
    }

    private static void createCall()
    {
        // Implement the method for creating calls here.
    }

    private static void createConfig()
    {
        // Implement the method for creating config here.
    }
}


//namespace DalTest;
//using DalApi;
//using DO;
//using System.Xml.Linq;

//public static class Initialization
//{
//    private static IVolunteer? s_dalVolunteer;
//    private static IConfig? s_dalConfig;
//    private static IAssignment? s_dalAssignment;
//    private static ICall? s_dalCall;

//    private static readonly Random s_rand = new();
//    private static void createVolunteer()
//    {
//        string[] names = {
//    "Roni", "Maya", "Lior", "Noa", "Dani",
//    "Tomer", "Liat", "Gal", "Yona", "Nir",
//    "Omer", "Shira", "Erez", "Michal", "Hadar"
//};

//        string[] emails = {
//    "Roni@gmail.com", "Maya@gmail.com", "Lior@gmail.com", "Noa@gmail.com", "Dani@gmail.com",
//    "Tomer@gmail.com", "Liat@gmail.com", "Gal@gmail.com", "Yona@gmail.com", "Nir@gmail.com",
//    "Omer@gmail.com", "Shira@gmail.com", "Erez@gmail.com", "Michal@gmail.com", "Hadar@gmail.com"
//};

//        string[] phones = {
//    "050-321-7845", "052-987-1243", "054-123-9876", "053-549-4567", "055-789-1234",
//    "050-987-6789", "054-112-7654", "053-785-6543", "052-312-7890", "055-654-3210",
//    "053-789-5647", "050-125-1234", "052-456-8765", "054-654-4321", "055-123-4567"
//};

//        string[] addresses = {
//    "Nazareth", "Haifa", "Beersheba", "Eilat", "Rosh Ha'ayin",
//    "Afula", "Kiryat Shmona", "Karmiel", "Dimona", "Tiberias",
//    "Safed", "Kiryat Gat", "Ashkelon", "Lod", "Ramat Gan"
//};
//        for (int i = 0; i < names.Length; i++)
//        {
//            int id;
//            do
//                while (s_dalVolunteer!.Read(id) != null) ;
//string name = names[i];
//            string email = e

//id = s_rand.Next(100000000, 999999999);

//            5_dalVolunteer!.Create(new());
//        }
//    }
//    private static void createCall();
//    private static void createConfig();
//}
