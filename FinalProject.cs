using System;
using System.Threading;
using System.Collections.Generic;

namespace FinalFinalFinal
{
    class UserManager
    {
        private static Dictionary<string, string> userPassword = new Dictionary<string, string>();

        public static void Register()
        {
            Console.Write("Enter username : ");
            string username = Console.ReadLine();
            if (userPassword.ContainsKey(username))
            {
                Color.WriteLine(ConsoleColor.Red, "Username already exists. Please choose a different username.");
                Thread.Sleep(2000);
                return;
            }

            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            userPassword.Add(username, password);
            Color.WriteLine(ConsoleColor.Green, "Registration successful.");
            Thread.Sleep(2000);

        }

        public static bool Login()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            if (userPassword.ContainsKey(username) && userPassword[username] == password)
            {
                Color.WriteLine(ConsoleColor.Green,"Login successful.");
                Thread.Sleep(2000);

                return true;
            }
            else
            {
                Color.WriteLine(ConsoleColor.Red, "Invalid username or password. Please try again.");
                Thread.Sleep(2000);

                return false;
            }
        }

    }

    class Color
    {
        public static void WriteLine(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }


    internal class Program
    {
        static void Main()
        {

            bool isLoggedin = false;
            string[] loginOptions = { "1.Register", "2.Login", "3.Exit" };

            while (!isLoggedin)
            {
                

                switch (choice)
                {
                    case 0:
                        UserManager.Register();
                        break;
                    case 1:
                        if (UserManager.Login())
                        {
                            isLoggedin = true;
                        }
                        break;
                    case 2:
                        Console.WriteLine("Goodbye!");
                        return;
                }
            }

            if (isLoggedin)
            {
                SecondPage();
            }

        }

        static void SecondPage()
        {
            string[] secondPageOptions = { "1.Check bus schedule", "2.Exit" }; 
            

            switch(choice)
            {
                case 0:
                    ThirdPage();
                    break;
                case 1:
                    return;
            }
        }

        static void ThirdPage()
        {
            Console.Clear();
            string[] dayOptions = { "Monday", "Wednesday", "Friday"};
            

            while (true)
            {
                Color.WriteLine(ConsoleColor.Green,"----------DAY SCHEDULE---------");
                Console.WriteLine();

                Color.WriteLine(ConsoleColor.Yellow, "Monday");
                Color.WriteLine(ConsoleColor.Green, "Wednesday");
                Color.WriteLine(ConsoleColor.Blue, "Friday");


                Console.WriteLine();

                string d;
                bool isDayValid = false;

                do
                {
                    Console.Write("Please reserve your date: ");

                    d = Console.ReadLine();
                    isDayValid = (d.ToLower() == "monday" || d.ToLower() == "wednesday" || d.ToLower() == "friday");

                    if (!isDayValid)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("");
                        Console.WriteLine("Invalid day. Please enter a valid day (Monday, Wednesday, or Friday).");
                        Console.WriteLine("");
                        Console.ForegroundColor = ConsoleColor.White;

                    }
                }
                while (!isDayValid);

                Console.WriteLine("");
                Color.WriteLine(ConsoleColor.Blue, "----------Timetable---------");
                Console.WriteLine("");
                Color.WriteLine(ConsoleColor.Gray, "     06:00     12:00     21:30");
                Console.WriteLine("");

                string t;
                bool isTimeValid = false;
                do
                {
                    Console.Write("Please reserve your time: ");

                    t = Console.ReadLine();
                    isTimeValid = (t == "06:00" || t == "12:00" || t == "21:30");

                    if (!isTimeValid)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("");
                        Console.WriteLine("Invalid time. There is no timetable you want. Please select again.");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("");
                    }
                }
                while (!isTimeValid);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Double check your date/time {0} {1} ", d, t);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("1 = Correct ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("2 = Wrong ");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write("Check your date/time.....");

                int c = Convert.ToInt32(Console.ReadLine());
                Console.ForegroundColor = ConsoleColor.White;
                if (c == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("");
                    Console.WriteLine("Reservation successful");
                    Console.ForegroundColor = ConsoleColor.White;
                    Thread.Sleep(1000);

                    Console.Clear();

                    //หน้า 4 เลือก ที่นั้ง
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Day/time : {0} / {1} ", d, t);
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("--------- Choose your seat --------- ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine();
                    Console.WriteLine("    1A empty             2B empty");
                    Console.WriteLine("    3C empty             4D empty");
                    Console.WriteLine("    5E empty             6F empty");
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    string ys;
                    bool s = false;

                    do
                    {
                        Console.Write("You choose your seat : ");
                        ys = Console.ReadLine();
                        s = (ys.ToLower() == "1a" || ys.ToLower() == "2b" || ys.ToLower() == "3c" || ys.ToLower() == "4d" || ys.ToLower() == "5e" || ys.ToLower() == "6f");

                        if (!s)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("");
                            Console.WriteLine("Invalid seat !!!! Please enter a seat again.");
                            Console.WriteLine("");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    while (!s);

                    confirmticket(ys);
                    return;
                }
                else if (c == 2)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("");
                    Console.WriteLine("Reservation wrong");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("");
                    Console.WriteLine("Done.....");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }
        }

        static void confirmticket(string ys)
        {
            while (true)
            {
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Seat is you choose is {0} ", ys);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("");
                Console.WriteLine("To confirm ticket purchase = press 1 ");
                Console.WriteLine("Exit                       = press 2 ");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Please press..... ");
                int ct = Convert.ToInt32(Console.ReadLine());

                if (ct == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("");
                    Console.WriteLine("Confirm ticket please wait....");
                    Console.ForegroundColor = ConsoleColor.White;
                    Thread.Sleep(1000);
                    Console.Clear();

                    //ไปต่อหน้า 5
                    Console.WriteLine("Thank you bra bra bra Ku Nguang Non Laeo ");
                    return;
                }
                else if (ct == 2)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("");
                    Console.WriteLine("Done.....");
                    Console.ForegroundColor = ConsoleColor.White;
                    Thread.Sleep(1000);
                    return;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("");
                    Console.WriteLine("They are have 2 option please press again !!!!!");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
