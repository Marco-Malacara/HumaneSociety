﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class UserInterface
    {
        public static void DisplayUserOptions(List<string> options)
        {
            foreach(string option in options)
            {
                Console.WriteLine(option);
            }
        }
        public static void DisplayUserOptions(string options)
        {
            Console.WriteLine(options);
        }
        public static string GetUserInput()
        {
            string input = Console.ReadLine();
            switch (input.ToLower())
            {
                case "reset":
                    Console.Clear();
                    PointOfEntry.Run();
                    Environment.Exit(1);
                    break;
                case "exit":
                    Environment.Exit(1);
                    break;
                default:
                    break;
            }

            return input;
        }
        public static string GetStringData(string parameter, string target)
        {
            string data;
            DisplayUserOptions($"What is {target} {parameter}?");
            data = GetUserInput();
            return data;
        }

        internal static bool? GetBitData(List<string> options)
        {
            DisplayUserOptions(options);
            string input = GetUserInput();
            if (input.ToLower() == "yes" || input.ToLower() == "y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool? GetBitData()
        {
            string input = GetUserInput();
            if (input.ToLower() == "yes" || input.ToLower() == "y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static bool? GetBitData(string target, string parameter)
        {
            DisplayUserOptions($"Is {target} {parameter}?");
            string input = GetUserInput();
            if (input.ToLower() == "yes" || input.ToLower() == "y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static void DisplayAnimals(List<Animal> animals)
        {
            foreach(Animal animal in animals)
            {
                Console.WriteLine(animal.AnimalId + " " + animal.Name + " " + animal.Specy.Name);
            }
        }

        internal static int GetIntegerData()
        {
            try
            {
                int data = int.Parse(GetUserInput());
                return data;
            }
            catch
            {
                Console.Clear();
                DisplayUserOptions("Incorrect input please enter an integer number.");
                return GetIntegerData();
            }
        }

        public static int GetIntegerData(string parameter, string target)
        {
            try
            {
                int data = int.Parse(GetStringData(parameter, target));
                return data;
            }
            catch
            {
                Console.Clear();
                DisplayUserOptions("Incorrect input please enter an integer number.");
                return GetIntegerData(parameter, target);
            }
        }

        public static void DisplayEmployees( IEnumerable<Employee> employees)
        {
            Console.WriteLine("Here are the current employees:");
            foreach(Employee employee in employees)
            {
                Console.WriteLine($"{employee.EmployeeId}: {employee.FirstName} {employee.LastName}");
            }
        }

        internal static void DisplayClientInfo(int clientId)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var client = db.Clients.Distinct().SingleOrDefault(Client => Client.ClientId == clientId);
            List<string> info = new List<string>() { client.FirstName, client.LastName, client.Email, "Number of kids: " + client.NumberOfKids.ToString(), "Home size: " + client.HomeSquareFootage.ToString(), "Income: " + client.Income.ToString(), client.Address.USState.Name };
            DisplayUserOptions(info);
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }

       

        public static string BoolToYesNo(bool? input)
        {
            if (input == true)
            {
                return "yes";
            }
            else
            {
                return "no";
            }
        }

        public static bool GetBitData(string option)
        {
            DisplayUserOptions(option);
            string input = GetUserInput();
            if (input.ToLower() == "yes" || input.ToLower() == "y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Dictionary<int, string> GetAnimalCriteria()
        {
            Dictionary<int, string> searchParameters = new Dictionary<int, string>();
            bool isSearching = true;
            while (isSearching)
            {
                Console.Clear();
                List<string> options = new List<string>() { "Select Search Criteria: (Enter number and choose finished when finished)", "1. Species", "2. Name", "3. Age", "4. Demeanor", "5. Kid friendly", "6. Pet friendly", "7. Weight", "8. ID", "9. Finished" };
                DisplayUserOptions(options);
                string input = GetUserInput();
                if (input.ToLower() == "9" || input.ToLower() == "finished")
                {
                    isSearching = false;
                    continue;
                }
                else
                {
                    searchParameters = EnterSearchCriteria(searchParameters, input);
                }
            }
            return searchParameters;
        }
        public static Dictionary<int, string> EnterSearchCriteria(Dictionary<int, string> searchParameters, string input)
        {
            Console.Clear();
            switch (input)
            {
                case "1":
                    searchParameters.Add(1, UserInterface.GetStringData("species", "the animal's"));
                    return searchParameters;
                case "2":
                    searchParameters.Add(2, UserInterface.GetStringData("name", "the animal's"));
                    return searchParameters;
                case "3":
                    searchParameters.Add(3, UserInterface.GetIntegerData("age", "the animal's").ToString());
                    return searchParameters;
                case "4":
                    searchParameters.Add(4, UserInterface.GetStringData("demeanor", "the animal's"));
                    return searchParameters;
                case "5":
                    searchParameters.Add(5, UserInterface.GetBitData("the animal", "kid friendly").ToString());
                    return searchParameters;
                case "6":
                    searchParameters.Add(6, UserInterface.GetBitData("the animal", "pet friendly").ToString());
                    return searchParameters;
                case "7":
                    searchParameters.Add(7, UserInterface.GetIntegerData("weight", "the animal's").ToString());
                    return searchParameters;
                case "8":
                    searchParameters.Add(8, UserInterface.GetIntegerData("room", "the animal's").ToString());
                    return searchParameters;
                case "9":
                    searchParameters.Add(9, UserInterface.GetIntegerData("ID", "the animal's").ToString());
                    return searchParameters;

                default:
                    UserInterface.DisplayUserOptions("Input not recognized please try agian");
                    return searchParameters;
            }
        }
    }
}
