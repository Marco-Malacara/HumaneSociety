using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    class Admin : User
    {
        public override void LogIn()
        {
            UserInterface.DisplayUserOptions("What is your password?");
            string password = UserInterface.GetUserInput();
            if (password.ToLower() != "poiuyt")
            {
                UserInterface.DisplayUserOptions("Incorrect password please try again or type exit");
            }
            else
            {
                RunUserMenus();
            }
        }

        protected override void RunUserMenus()
        {
            Console.Clear();
            List<string> options = new List<string>() { "Admin log in successful.", "What would you like to do?", "1. Create new employee", "2. Delete employee", "3. Read employee info ", "4. Update emplyee info", "5. Import csv file of new animals", "(type 1, 2, 3, 4, 5,  create, read, update, delete, or import)" };
            UserInterface.DisplayUserOptions(options);
            string input = UserInterface.GetUserInput();
            RunInput(input);
        }
        protected void RunInput(string input)
        {
            if(input == "1" || input.ToLower() == "create")
            {
                AddEmployee();
                RunUserMenus();
            }
            else if(input == "2" || input.ToLower() == "delete")
            {
                RemoveEmployee();
                RunUserMenus();
            }
            else if(input == "3" || input.ToLower() == "read")
            {
                ReadEmployee();
                RunUserMenus();
            }
            else if (input == "4" || input.ToLower() == "update")
            {
                UpdateEmployee();
                RunUserMenus();
            }
            else if (input == "5" || input.ToLower() == "import")
            {
                ImportCSVFile();
                RunUserMenus(); 
            }
            else
            {
                UserInterface.DisplayUserOptions("Input not recognized please try again or type exit");
                RunUserMenus();
            }
        }

        private void ImportCSVFile()
        {
            Console.WriteLine("Enter name of .csv file to import:");
            string file = Console.ReadLine();
            bool shouldImportFile = Query.CheckIfCSVFileValid(file);
            if (shouldImportFile == true)
            {
                Query.ReadCSVFile(file);
                Console.WriteLine("Import finished. Press Enter to continue.");
            }
            else
            {
                Console.WriteLine("File path was not found. Press Enter to continue.");
            }
            Console.ReadLine();
            Console.Clear();
        }

        private void UpdateEmployee()
        {
            List<Employee> employees = Query.GetEmployees().ToList();
            UserInterface.DisplayEmployees(employees);
            int employeeid = UserInterface.GetIntegerData("id number", "the employee's");
            Employee employee = Query.GetEmployeeById(employeeid);
            try
            {
                Query.RunEmployeeQueries(employee, "update");
                UserInterface.DisplayUserOptions("Employee update successful.");
            }
            catch
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Employee update unsuccessful please try again or type exit;");
                return;
            }
        }

        private void ReadEmployee()
        {
            try
            {
                List<Employee> employees = Query.GetEmployees().ToList();
                UserInterface.DisplayEmployees(employees);
                int employeeid = UserInterface.GetIntegerData("id number", "the employee's");
                Employee employee = Query.GetEmployeeById(employeeid);
                Query.RunEmployeeQueries(employee, "read");
            }
            catch
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Employee not found please try again or type exit;");
                return;
            }
        }

        private void RemoveEmployee()
        {
            List<Employee> employees = Query.GetEmployees().ToList();
            UserInterface.DisplayEmployees(employees);
            int employeeid = UserInterface.GetIntegerData("id number", "the employee's");
            Employee employee = Query.GetEmployeeById(employeeid);
            try
            {
                Console.Clear();
                Query.RunEmployeeQueries(employee, "delete");
                UserInterface.DisplayUserOptions("Employee successfully removed");
            }
            catch
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Employee removal unsuccessful please try again or type exit");
                RemoveEmployee();
            }
        }

        private void AddEmployee()
        {
            Employee employee = new Employee();
            employee.FirstName = UserInterface.GetStringData("first name", "the employee's");
            employee.LastName = UserInterface.GetStringData("last name", "the employee's");
            employee.EmployeeNumber = UserInterface.GetIntegerData("employee number", "the employee's");
            employee.Email = UserInterface.GetStringData("email", "the employee's"); ;
            try
            {
                Query.RunEmployeeQueries(employee, "create");
                UserInterface.DisplayUserOptions("Employee addition successful.");
            }
            catch
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Employee addition unsuccessful please try again or type exit;");
                return;
            }
        }

    }
}
