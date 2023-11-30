using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace FinalProject
{
    [Serializable]
    public class ReservationData
    {
        public string Day { get; set; }
        public string Time { get; set; }
        public string[] ReservedSeats { get; set; }
    }

    [Serializable]
    public class UserData
    {
        public Dictionary<string, string> UserPassword { get; set; }
       
        public Dictionary<string, ReservationData> UserReservations { get; set; }

        public UserData() 
        {
            UserPassword = new Dictionary<string, string>();
            UserReservations = new Dictionary<string, ReservationData>();
        }
    }

    class UserManager
    {
        private static UserData userData = new UserData();
        private const string UserDataFileName = @"..\..\..\userdata.json";

        private static string userId;
        public static string UserId { get { return userId; } }

        static UserManager()
        {
            LoadUserData();
        }

        private static void LoadUserData()
        {
            try
            {
                string json = File.ReadAllText(UserDataFileName);
                userData = JsonSerializer.Deserialize<UserData>(json);
            }
            catch (FileNotFoundException)
            {
                
            }
        }

        private static void SaveUserData()
        {
            string json = JsonSerializer.Serialize(userData);
            File.WriteAllText(UserDataFileName, json);
        }

        public static ReservationData GetReservationData()
        {
            if (userData.UserReservations.TryGetValue(userId, out ReservationData reservationData))
            {
                return reservationData;
            }

            return new ReservationData();
        }

        public static void UpdateReservation(string[] selectedSeats, string selectedDay, string selectedTime)
        {
            if (userData.UserReservations.TryGetValue(userId, out ReservationData reservationData))
            {
                reservationData.Day = selectedDay;
                reservationData.Time = selectedTime;
                reservationData.ReservedSeats = selectedSeats;
                SaveUserData();
            }
            else
            {
                userData.UserReservations.Add(userId, new ReservationData());
                reservationData.Day = selectedDay;
                reservationData.Time = selectedTime;
                reservationData.ReservedSeats = selectedSeats;
                SaveUserData();

            }
        }
        public static void CancelReservation()
        {
            if (userData.UserReservations.TryGetValue(userId, out ReservationData reservationData))
            {
                reservationData.Day = null;
                reservationData.Time = null;
                reservationData.ReservedSeats = null;
                SaveUserData();
            }
            else
            {
                Color.WriteLine(ConsoleColor.Red, "\nYou don't have any reservation to cancel.\nPress any key to continue...");
                Console.ReadKey(true);
            }
        }

        public static void Register()
        {
            Console.CursorVisible = true;

            Console.Write("\nEnter username : ");
            string username = Console.ReadLine();
            Console.CursorVisible = false;

            if (string.IsNullOrWhiteSpace(username) || ContainsSpecialCharacters(username))
            {
                Color.WriteLine(ConsoleColor.Red, "\nInvalid username. Please choose a valid username without special characters or blank spaces.\nPress any key to try again...");
                Console.ReadKey(true);
                return;
            }

            if (userData.UserPassword.ContainsKey(username))
            {
                Color.WriteLine(ConsoleColor.Red, "\nUsername already exists. Please choose a different username.\nPress any key to try again...");
                Console.ReadKey(true);
                return;
            }

            Console.Write("Enter password : ");
            string password = Console.ReadLine();
            Console.CursorVisible = false;

            if (string.IsNullOrWhiteSpace(password) || ContainsSpecialCharacters(password))
            {
                Color.WriteLine(ConsoleColor.Red, "\nInvalid password. Please choose a valid password without special characters or blank spaces.\nPress any key to try again...");
                Console.ReadKey(true);
                return;
            }

            userData.UserPassword.Add(username, password);
            userData.UserReservations.Add(username, new ReservationData());
            SaveUserData();
            Console.CursorVisible = false;

            Color.WriteLine(ConsoleColor.Green, "\nRegistration successful. \nPress any key to continue...");
            Console.ReadKey(true);
        }


        public static bool Login()
        {
            Console.CursorVisible = true;
            Console.Write("\nEnter username : ");
            string username = Console.ReadLine();
            Console.CursorVisible = false;

            if (string.IsNullOrWhiteSpace(username) || ContainsSpecialCharacters(username))
            {
                Color.WriteLine(ConsoleColor.Red, "\nInvalid username. Please enter a valid username without special characters or blank spaces.\nPress any key to try again...");
                Console.ReadKey(true);
                return false;
            }


            Console.Write("Enter password : ");
            string password = Console.ReadLine();
            Console.CursorVisible = false;

            if (string.IsNullOrWhiteSpace(password) || ContainsSpecialCharacters(password))
            {
                Color.WriteLine(ConsoleColor.Red, "\nInvalid password. Please enter a valid password without special characters or blank spaces.\nPress any key to try again...");
                Console.ReadKey(true);
                return false;
            }

            if (userData.UserPassword.ContainsKey(username) && userData.UserPassword[username] == password)
            {
                userId = username;
                Color.WriteLine(ConsoleColor.Green,"\nLogin successful. \nPress any key to continue...");
                Console.ReadKey(true);
                return true;
            }
            else
            {
                Color.WriteLine(ConsoleColor.Red, "\nInvalid username or password. Please try again. \nPress any key to continue...");
                Console.ReadKey(true);
                return false;
            }
        }

        private static bool ContainsSpecialCharacters(string input)
        {
            return !Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");
        }

        public static void Logout()
        {
            userData.UserReservations[userId] = new ReservationData();
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

        public static void Write(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
    }

    class Menu
    {
        private int SelectedDayChoice,SelectedTimeChoice;
        private string[] Options;
        private string Title;
        private bool isHorizontalMode;

        private string[,] Seats;
        private string[] selectedSeats = new string[0];
        private int selectedRow, selectedColumn;
        private string SelectedDay, SelectedTime;


        private Dictionary<string, List<string>> Schedule;

        public Menu(string[] options,string title = null ,bool horizontalMode = false)
        {
            Options = options;
            Title = title;
            SelectedDayChoice = 0;
            SelectedTimeChoice = 0;
            isHorizontalMode = horizontalMode;
        }

        public Menu(Dictionary<string, List<string>> schedule, string title = null)
        {
            Schedule = schedule;
            Options = schedule.Keys.ToArray();
            Title = title;
            SelectedDayChoice = 0;
            SelectedTimeChoice = 0;

        }

        public Menu(string[,] seats, string title = null)
        {
            Seats = seats;
            Title = title;
            selectedRow = 0;
            selectedColumn = 0;
        }

        private void DisplayOptions(bool isScheduleTableMode = false,int selectedDay = -1)
        {
            if (Title != null)
            {
                Color.WriteLine(ConsoleColor.Yellow,Title+"\n");
            }

            for (int currentChoiceIndex = 0; currentChoiceIndex < Options.Length; currentChoiceIndex++)
            {
                string currentOption = Options[currentChoiceIndex];
                string pointer;

                if (SelectedDayChoice == currentChoiceIndex)
                {
                    pointer = "<--";
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                else
                {
                    pointer = "   ";
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                if (isHorizontalMode || isScheduleTableMode)
                {
                    Console.Write($" {currentOption} ");

                    if (isScheduleTableMode && selectedDay < 0)
                    {
                        if (Schedule.TryGetValue(currentOption, out List<string> times))
                        {
                            Console.ResetColor();
                            Color.WriteLine(ConsoleColor.DarkGray,"\t" + string.Join("\t", times));
                        }
                        Console.ResetColor();

                    }

                    if (isScheduleTableMode && selectedDay > -1 )
                    {
                        if (Schedule.TryGetValue(currentOption, out List<string> times))
                        {
                            for (int currentTimeIndex = 0; currentTimeIndex < times.Count; currentTimeIndex++)
                            {
                                if (selectedDay == currentChoiceIndex && SelectedTimeChoice == currentTimeIndex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.BackgroundColor = ConsoleColor.White;
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.BackgroundColor = ConsoleColor.Black;
                                }

                                if(selectedDay != currentChoiceIndex)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                }

                                Console.Write("\t" + times[currentTimeIndex]);
                            }

                            Console.ResetColor();
                            Console.Write("\n");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($" {currentOption} ");
                }

                
            }
            Console.ResetColor();

            if (isScheduleTableMode)
            {
                Color.WriteLine(ConsoleColor.Cyan, "\nUse ARROW keys to navigate / Press ENTER to select\n");
            }
        }

        public int SelectOption(bool isScheduleTableMode = false, int selectedDay = -1)
        {
            ConsoleKey keyPressed;
            do
            {
                Console.Clear();
                DisplayOptions(isScheduleTableMode,selectedDay);

                keyPressed = Console.ReadKey(true).Key;

                NavigateOptionKey(keyPressed,selectedDay);
                

            } while (keyPressed != ConsoleKey.Enter);


            if (selectedDay < 0)
            {
                return SelectedDayChoice;
            }
            else
            {
                return SelectedTimeChoice;
            }
        }

        public int SelectMenu(ReservationData reservationData)
        {
            ConsoleKey keyPressed;
            do
            {
                Console.Clear();

                Console.WriteLine("Your Reservation Details");
                Color.WriteLine(ConsoleColor.Yellow, $"Day: {reservationData.Day} / Time: {reservationData.Time}");

                Console.Write("\nReserved Seats : ");
                foreach (string seat in reservationData.ReservedSeats)
                {
                    Color.Write(ConsoleColor.Blue, $"{seat} ");
                }

                Console.WriteLine("\n");
                DisplayOptions();

                keyPressed = Console.ReadKey(true).Key;

                NavigateOptionKey(keyPressed);


            } while (keyPressed != ConsoleKey.Enter);


            return SelectedDayChoice;
        }

        private void NavigateOptionKey(ConsoleKey keyPressed, int selectedDay = -1)
        {
            if (keyPressed == ConsoleKey.UpArrow || keyPressed == ConsoleKey.LeftArrow)
            {
                if (selectedDay < 0)
                {
                    SelectedDayChoice--;

                    if (SelectedDayChoice < 0)
                    {
                        SelectedDayChoice = Options.Length - 1;
                    } 
                }
                else
                {
                    SelectedTimeChoice--;

                    if (SelectedTimeChoice < 0)
                    {
                        SelectedTimeChoice = Schedule[Options[selectedDay]].Count - 1;
                    }
                }
            }
            else if (keyPressed == ConsoleKey.DownArrow || keyPressed == ConsoleKey.RightArrow)
            {
                if (selectedDay < 0)
                {
                    SelectedDayChoice++;

                    if (SelectedDayChoice >= Options.Length)
                    {
                        SelectedDayChoice = 0;
                    } 
                }
                else
                {
                    SelectedTimeChoice++;

                    if (SelectedTimeChoice >= Schedule[Options[selectedDay]].Count)
                    {
                        SelectedTimeChoice = 0;
                    }
                }
            }
        }

        private void DisplaySeats()
        {
            if (Title != null)
            {
                Color.WriteLine(ConsoleColor.Yellow,Title+"\n");
            }
            int maxSeatIdLength = Seats.Cast<string>().Max(s => s.Length);
            for (int i = 0; i < Seats.GetLength(0); i++)
            {
                for (int j = 0; j < Seats.GetLength(1); j++)
                {
                    string seatId = Seats[i, j];
                    string seatDisplay = $"{seatId.PadRight(maxSeatIdLength)}";

                    bool isSelected = selectedSeats.Contains(seatId);
                    bool isFocused = i == selectedRow && j == selectedColumn;
                    bool isReserved = IsSeatReserved(SelectedDay, SelectedTime, seatId);

                    if (isSelected)
                    {
                        if (isFocused)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.Green;
                        }
                    }
                    else if (isFocused)
                    {
                        if (isReserved)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.Red;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.White;
                        }
                    }
                    else if (isReserved)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    Console.Write($"[{seatDisplay}]");

                    Console.ResetColor();

                    if ((j + 1) % 2 == 0 && j < Seats.GetLength(1) - 1)
                    {
                        Console.Write("  ");
                    }

                    Console.Write(" ");
                }
                Console.WriteLine("\n");
            }
            Color.WriteLine(ConsoleColor.Cyan, "\nUse ARROW keys to navigate / Press SPACE BAR to select\nPress ENTER to confirm \n");
        }

        public string[] SelectSeats(string selectedDay, string selectedTime)
        {
            SelectedDay = selectedDay;
            SelectedTime = selectedTime;

            ConsoleKey keyPressed;
            do
            {
                
                Console.Clear();
                DisplaySeats();

                keyPressed = Console.ReadKey(true).Key;

                NavigateSeatKey(keyPressed);


            } while (keyPressed != ConsoleKey.Enter);

            if (selectedSeats.Length == 0)
            {
                return SelectSeats(selectedDay,selectedTime);
            }

            return selectedSeats;

        }

        private void NavigateSeatKey(ConsoleKey KeyPressed)
        {
            string currentSeat = Seats[selectedRow, selectedColumn];

            bool isCurrentSeatReserved = IsSeatReserved(SelectedDay, SelectedTime, currentSeat);

            if (KeyPressed == ConsoleKey.UpArrow && selectedRow > 0)
            {
                selectedRow--;
            }
            else if (KeyPressed == ConsoleKey.DownArrow && selectedRow < Seats.GetLength(0) - 1)
            {
                selectedRow++;
            }
            else if (KeyPressed == ConsoleKey.LeftArrow && selectedColumn > 0)
            {
                selectedColumn--;
            }
            else if (KeyPressed == ConsoleKey.RightArrow && selectedColumn < Seats.GetLength(1) - 1)
            {
                selectedColumn++;
            }
            else if (KeyPressed == ConsoleKey.Spacebar && !isCurrentSeatReserved)
            {
                string selectedSeat = Seats[selectedRow, selectedColumn];

                if (selectedSeats.Contains(selectedSeat))
                {
                    selectedSeats = selectedSeats.Where(seat => seat != selectedSeat).ToArray();
                }
                else
                {
                    selectedSeats = selectedSeats.Concat(new[] { selectedSeat }).ToArray();
                }
            }
        }

        private bool IsSeatReserved(string selectedDay, string selectedTime, string seatId)
        {
            ReservationData reservationData = UserManager.GetReservationData();

            if (reservationData != null && reservationData.Day == selectedDay && reservationData.Time == selectedTime)
            {
                return reservationData.ReservedSeats.Contains(seatId);
            }

            return false;
        }
    }

    public enum AppState
    {
        Login,
        MainMenu,
        CheckSchedule,
        SeatReseravation,
        Confirmation,
        CheckReservation,
        Exit
    }

    internal class Program
    {
        private static AppState currentState;

        private static string selectedDay;
        private static string selectedTime;
        private static string[] selectedSeats;

        private static double totalPrice;
        private  const double pricePerSeat = 200;

        private static readonly Dictionary<string, List<string>> schedule = new Dictionary<string, List<string>>
        {
            { "Monday"   , new List<string> { "09:00", "12:00", "21:30"} },
            { "Wednesday", new List<string> { "09:00", "12:00", "21:30"} },
            { "Friday"   , new List<string> { "09:00", "12:00", "21:30"} },
        };
        private static readonly string[,] seats = {
            { "1A", "1B", "1C", "1D" },
            { "2A", "2B", "2C", "2D" },
            { "3A", "3B", "3C", "3D" },
            { "4A", "4B", "4C", "4D" },
            { "5A", "5B", "5C", "5D" },
            { "6A", "6B", "6C", "6D" },
            { "7A", "7B", "7C", "7D" },
            { "8A", "8B", "8C", "8D" }
        };

        static void Main()
        {
            Console.Title = "Bus Schedule Booking App";
            Console.CursorVisible = false;

            currentState = AppState.Login;

            while (currentState != AppState.Exit)
            {
                switch (currentState)
                {
                    case AppState.Login:
                        FirstPage();
                        break;

                    case AppState.MainMenu:
                        SecondPage(); 
                        break;

                    case AppState.CheckSchedule:
                        ThirdPage();
                        break;

                    case AppState.SeatReseravation:
                        FourthPage();
                        break;

                    case AppState.Confirmation:
                        FifthPage();
                        break;

                    case AppState.CheckReservation:
                        SixthPage();
                        break;
                }
            }

            Color.WriteLine(ConsoleColor.DarkYellow,"\nThanks for visit our app");
        }

        static void FirstPage()
        {
            bool isLoggedin = false,isExit = false;
            string[] loginOptions = { "1.Register", "2.Login", "3.Exit" };

            while (!isLoggedin && !isExit)
            {

                Menu loginMenu = new Menu(loginOptions, "Register Menu");
                int choice = loginMenu.SelectOption();

                switch (choice)
                {
                    case 0:
                        UserManager.Register();
                        break;

                    case 1:
                        if (UserManager.Login())
                        {
                            isLoggedin = true;
                            currentState = AppState.MainMenu;
                        }
                        break;

                    case 2:
                        isExit = true;
                        currentState = AppState.Exit;
                        break;

                }
            }
            return;
        }

        static void SecondPage()
        {
            string[] secondPageOptions = { "1.Check bus schedule", "2.View reservations","3.Logout", "4.Exit" }; 
            Menu secondPageMenu = new Menu(secondPageOptions, "Choose your options"); 
            int choice = secondPageMenu.SelectOption();

            switch(choice)
            {
                case 0:
                    currentState = AppState.CheckSchedule;
                    break;

                case 1:
                    currentState = AppState.CheckReservation; 
                    break;

                case 2:
                    currentState = AppState.Login;
                    break;

                case 3:
                    currentState = AppState.Exit;
                    break;
            }

            return;
        }

        static void ThirdPage()
        {
            ConsoleKey key;
            Menu scheduleMenu = new Menu(schedule,"------------ DAY SCHEDULE -----------");

            do
            {
                Console.Clear();

                int daySelection = scheduleMenu.SelectOption(true);
                selectedDay = schedule.Keys.ToArray()[daySelection];

                int timeSelection = scheduleMenu.SelectOption(true, daySelection);
                selectedTime = schedule.Values.ToArray()[daySelection][timeSelection];

                Console.Write($"Your selection is ");
                Color.WriteLine(ConsoleColor.DarkYellow, $"{selectedDay} / {selectedTime} ");

                Console.WriteLine("Press ENTER to continue / Press ESC to undo");

                do { key = Console.ReadKey(true).Key;
                } while (key != ConsoleKey.Enter && key != ConsoleKey.Escape);
            } while (key != ConsoleKey.Enter);

            currentState = AppState.SeatReseravation;
            return;
        }

        static void FourthPage()
        {
            ConsoleKey key;
            Menu seatsMenu = new Menu(seats, "--------- SEAT RESERVE ---------\n" + $"\nDay / Time : {selectedDay} / {selectedTime}\n");

            do
            {
                Console.Clear();

                selectedSeats = seatsMenu.SelectSeats(selectedDay,selectedTime);
                Console.Write(selectedSeats.Length > 1 ? "Your seats are : " : "Your seat is : ");
                foreach ( string seat in selectedSeats)
                {
                    Color.Write(ConsoleColor.Blue, $"{seat} ");
                }

                totalPrice = selectedSeats.Length * pricePerSeat;
                Console.Write($"\nTotal price : ");
                Color.WriteLine(ConsoleColor.DarkGreen, $" {totalPrice} ฿");

                Console.WriteLine("\nPress ENTER to continue / Press ESC to undo");

                do { key = Console.ReadKey(true).Key;
                } while (key != ConsoleKey.Enter && key != ConsoleKey.Escape);
            } while (key != ConsoleKey.Enter);

            currentState = AppState.Confirmation;
            return;
        }

        static void FifthPage()
        {
            Console.Clear();

            Console.WriteLine("Reservation Details");
            Color.WriteLine(ConsoleColor.Yellow, $"Day: {selectedDay} / Time: {selectedTime}");

            Console.Write("Selected Seats : ");
            foreach (string seat in selectedSeats)
            {
                Color.Write(ConsoleColor.Blue, $"{seat} ");
            }

            Console.Write($"\nTotal price : ");
            Color.WriteLine(ConsoleColor.DarkGreen, $" {totalPrice} ฿");

            bool paymentSuccessful = false;
            do
            {
                Console.Write("Enter amount to pay :");
                string userInput = Console.ReadLine();

                if (double.TryParse(userInput, out double userPaid))
                {
                    if (userPaid >= totalPrice)
                    {
                        double change = userPaid - totalPrice;
                        Console.WriteLine($"Your change : {change} ฿");
                        Color.WriteLine(ConsoleColor.Green, $"Payment successful!");
                        paymentSuccessful = true;
                    }
                    else
                    {
                        Color.WriteLine(ConsoleColor.Red, "Insufficient payment. Please try again.");
                    }
                }
                else
                {
                    Color.WriteLine(ConsoleColor.Red, "Invalid input. Please enter a valid numeric amount.");
                } 
            } while (!paymentSuccessful);

            Console.WriteLine("Thank you for using on service. Have a safe trip♥");
            UserManager.UpdateReservation(selectedSeats, selectedDay, selectedTime);
            Console.WriteLine("Press any button to back to main menu...");
            Console.ReadKey(true);

            currentState = AppState.MainMenu;
            return;
        }

        static void SixthPage()
        {
            ReservationData reservationData = UserManager.GetReservationData();

            Console.Clear();

            if (reservationData != null && !string.IsNullOrEmpty(reservationData.Day))
            {
                string[] reservationDataOptions = { "1. Cancel reservation", "2. Go back" };
                Menu menu = new Menu(reservationDataOptions);

                

                int choice = menu.SelectMenu(reservationData);

                switch (choice)
                {
                    case 0:
                        UserManager.CancelReservation();

                        Color.WriteLine(ConsoleColor.Green,"Your reservation have canceled successfully. \nPress any key to continue...");
                        Console.ReadKey(true);
                        currentState = AppState.MainMenu;
                        break;

                    case 1:
                        currentState = AppState.MainMenu;
                        break;
                }

            }
            else
            {
                Color.WriteLine(ConsoleColor.Yellow, "You don't have any reservation yet.\nPress any key to go back to main menu...");
                Console.ReadKey(true);
                currentState = AppState.MainMenu;
            }
        }
    }
}