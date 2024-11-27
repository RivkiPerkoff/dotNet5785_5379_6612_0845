using System;

namespace targil0;

partial class Program
{
    private static void Main(string[] args)
    {
        Welcome5379();
        Welcome6612();
        Welcome0845();

        Console.ReadKey();
       

    }

    static partial void Welcome6612();
    static partial void Welcome0845();

        private static void Welcome5379()
        {
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine()!;
            Console.WriteLine($"{name}, welcome to my first console application");
        }

}
