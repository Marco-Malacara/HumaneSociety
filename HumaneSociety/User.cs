using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    class User
    {
        protected string name;
        protected string userName;

        public virtual void LogIn()
        {

        }
        protected bool CheckIfNewUser()
        {
            List<string> options = new List<string>() { "Are you a new User?", "yes", "no" };
            UserInterface.DisplayUserOptions(options);
            string input = UserInterface.GetUserInput();
            if(input.ToLower() == "yes" || input.ToLower() == "y")
            {
                return true;
            }
            else if(input.ToLower() == "no" || input.ToLower() == "n")
            {
                return false;
            }
            else
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Input not recognized please try again");
                return CheckIfNewUser();
            }
        }
        protected virtual void LogInPreExistingUser()
        {

        }
        protected virtual void RunUserMenus()
        {

        }
                 
    }
}
