using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query 
    {
        public delegate void EmployeeToVoidFunction(Employee employee);

        public static void UpdateAdoption(bool isApproved, Adoption adoption)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var adoptionToUpdate = db.Adoptions.Single(a => adoption.AdoptionId == a.AdoptionId);
            switch (isApproved)
            {
                case true:
                    adoptionToUpdate.ApprovalStatus = "approved";
                    break;
                case false:
                    adoptionToUpdate.ApprovalStatus = "available";
                    break;
            }
            db.SubmitChanges();
        }
        
        public static IQueryable<Adoption> GetPendingAdoptions()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var pendingAdoptions = db.Adoptions.Select(Adoption => Adoption).Where(Adoption => Adoption.ApprovalStatus.ToLower() == "pending");
            return pendingAdoptions;
        }

        public static void UpdateShot(string shotType, Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalToUpdate = db.AnimalShots.AsEnumerable().Join(db.Shots.AsEnumerable(), AnimalShot => AnimalShot.ShotId, Shot => Shot.ShotId, (AnimalShot, Shot) => new
            {
                AnimalShot,
                Shot
            }).Select(Animal => Animal).Where(Animal => Animal.AnimalShot.AnimalId == animal.AnimalId);
            animalToUpdate.Select(Animal => Animal.Shot.Name = shotType);
            animalToUpdate.Select(Animal => Animal.AnimalShot.DateReceived = DateTime.Now);
            db.SubmitChanges();

        }

        public static IEnumerable<AnimalShot> GetShots(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var shots = db.AnimalShots.Join(db.Animals.AsEnumerable(), AnimalShot => AnimalShot.AnimalId, Animal => Animal.AnimalId, (AnimalShot, Animal) => new { AnimalShot, Animal }).Select(a => a).Where(a => a.AnimalShot.AnimalId == a.Animal.AnimalId).Cast<AnimalShot>();
            return shots;
        }

        public static IEnumerable<Animal> SearchForAnimalByMultipleTraits(Dictionary<int,string> searchParameters)
        {

            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            var animals = from data in db.Animals select data;

            if (searchParameters.ContainsKey(1))
            {
                animals = (from animal in animals where animal.Specy.Name == searchParameters[1] select animal);
            }
            if (searchParameters.ContainsKey(2))
            {
                animals = (from animal in animals where animal.Name == searchParameters[2] select animal);
            }
            if (searchParameters.ContainsKey(3))
            {
                animals = (from animal in animals where animal.Age == int.Parse(searchParameters[3]) select animal);
            }
            if (searchParameters.ContainsKey(4))
            {
                animals = (from animal in animals where animal.Demeanor == searchParameters[4] select animal);
            }
            if (searchParameters.ContainsKey(5))
            {
                bool parameter = false;
                if (searchParameters[5].ToLower().Trim() == "true" || searchParameters[5].ToLower().Trim() == "yes" || searchParameters[5] == "1")
                {
                    parameter = true;
                }
                else if (searchParameters[5].ToLower().Trim() == "false" || searchParameters[5].ToLower().Trim() == "no" || searchParameters[5] == "0")
                {
                    parameter = false;
                }
                else
                {
                    //error handling
                }
                animals = (from animal in animals where animal.KidFriendly == parameter select animal);
            }
            if (searchParameters.ContainsKey(6))
            {
                bool parameter = false;
                if (searchParameters[6].ToLower().Trim() == "true" || searchParameters[6].ToLower().Trim() == "yes" || searchParameters[6] == "1")
                {
                    parameter = true;
                }
                else if (searchParameters[6].ToLower().Trim() == "false" || searchParameters[6].ToLower().Trim() == "no" || searchParameters[6] == "0")
                {
                    parameter = false;
                }
                else
                {
                    //error handling
                }
                animals = (from animal in animals where animal.PetFriendly == parameter select animal);
            }
            if (searchParameters.ContainsKey(7))
            {
                animals = (from animal in animals where animal.Weight == int.Parse(searchParameters[7]) select animal);
            }
            if (searchParameters.ContainsKey(8))
            {
                animals = (from animal in animals where animal.AnimalId == int.Parse(searchParameters[8]) select animal);
            }
            return animals;

        }

        public static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Employee newEmployee = new Employee
            {
                EmployeeNumber = employeeNumber,
                Email = email
            };

            db.Employees.InsertOnSubmit(newEmployee);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception)
            {
                Console.WriteLine("The new Employee could not be added to the data base!");
            }
            return newEmployee;
        }

        public static Employee EmployeeLogin(string userName, string password)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var employee = db.Employees.Single(info => userName == info.UserName && password == info.Password);
            return employee;
        }

        public static void EnterUpdate(Animal animal, Dictionary<int, string> updates)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalToUpdate = db.Animals.Single(a => a.AnimalId == animal.AnimalId);
            if (updates.ContainsKey(1))
            {
                var possibleSpecies = db.Species.SingleOrDefault(s => updates[1].ToLower().Trim() == s.Name.ToLower());
                if (possibleSpecies == null)
                {
                    throw new Exception("Species could not be updated because a valid species was not found.");
                    //function to create new species?
                }
                else
                {
                    animalToUpdate.SpeciesId = possibleSpecies.SpeciesId;
                }   
            }
            if (updates.ContainsKey(2))
            {
                animalToUpdate.Name = updates[2];
            }
            if (updates.ContainsKey(3))
            {
                int possibleAge;
                bool isAge = Int32.TryParse(updates[3], out possibleAge);
                if (isAge == true)
                {
                    animalToUpdate.Age = possibleAge;
                }
                else
                {
                    throw new Exception("Age could not be updated because a valid number was not entered.");
                }  
            }
            if (updates.ContainsKey(4))
            {
                if (updates[4].ToLower().Trim() == "aggressive" || updates[4].ToLower().Trim() == "passive" || updates[4].ToLower().Trim() == "friendly")
                {
                    animalToUpdate.Demeanor = updates[4].ToLower().Trim();
                }
                else
                {
                    throw new Exception("Demeanor was not updated because the new demeanor entered was not valid.");
                }
            }    
            if (updates.ContainsKey(5))
            {
                if (updates[5].ToLower().Trim() == "true" || updates[5].Trim() == "1" || updates[5].Trim().ToLower() == "yes")
                {
                    animalToUpdate.KidFriendly = true;
                }
                else if (updates[5].ToLower().Trim() == "false" || updates[5].Trim() == "0" || updates[5].Trim().ToLower() == "no")
                {
                    animalToUpdate.KidFriendly = false;
                }
                else
                {
                    throw new Exception("Kid friendly value was not updated because a valid value was not entered.");
                }
            }
            if (updates.ContainsKey(6))
            {
                if (updates[6].ToLower().Trim() == "true" || updates[6].Trim() == "1" || updates[6].Trim().ToLower() == "yes")
                {
                    animalToUpdate.PetFriendly = true;
                }
                else if (updates[6].ToLower().Trim() == "false" || updates[6].Trim() == "0" || updates[6].Trim().ToLower() == "no")
                {
                    animalToUpdate.PetFriendly = false;
                }
                else
                {
                    throw new Exception("Pet friendly value was not updated because a valid value was not entered.");
                }
            }
            if (updates.ContainsKey(7))
            {
                int possibleWeight;
                bool isWeight = Int32.TryParse(updates[7], out possibleWeight);
                if (isWeight == true)
                {
                    animalToUpdate.Weight = possibleWeight;
                }
                else
                {
                    throw new Exception("Weight could not be updated because a valid number was not entered.");
                }
            }
            if (updates.ContainsKey(8))
            {
                throw new Exception("Id cannot be changed.");
            }
            db.SubmitChanges();
        }

        public static void RemoveAnimal(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();

        }
        public static Specy GetSpecies()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            string speciesName = UserInterface.GetStringData("the animal's", "species");
            if (!NameIsInSpeciesTable(db, speciesName))
            {
                db.Species.InsertOnSubmit(new Specy() { Name = speciesName });
                db.SubmitChanges();
            }
            var selectedSpecy = db.Species.Distinct().Select(Specy => Specy).Where(Specy => Specy.Name.ToLower() == speciesName.ToLower());
            return selectedSpecy as Specy;
        }

        public static DietPlan GetDietPlan()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            string dietPlanName = UserInterface.GetStringData("the animal's", "diet plan");
            if (!NameIsInDietPlanTable(db, dietPlanName))
            {
                db.DietPlans.InsertOnSubmit(new DietPlan() { Name = dietPlanName,
                                                             FoodType = UserInterface.GetStringData("the diet plan's", "food type"),
                                                             FoodAmountInCups = UserInterface.GetIntegerData("the diet plan's", "amount in cups")
                });
                db.SubmitChanges();
            }
            var selectedDietPlan = db.DietPlans.Distinct().Select(DietPlan => DietPlan).Where(DietPlan => DietPlan.Name.ToLower() == dietPlanName.ToLower());
            return selectedDietPlan as DietPlan;
        }

        public static void AddAnimal(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }
        public static bool CheckEmployeeUserNameExist(string userName)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            
            if(db.Employees.Single(user => userName == user.UserName) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void AddUsernameAndPassword(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            db.Employees.InsertOnSubmit(employee);
            db.SubmitChanges();
        }
       
        private static bool NameIsInSpeciesTable (HumaneSocietyDataContext database ,string stringToCompare )
        {
            return database.Species.Distinct().SingleOrDefault(Specy => Specy.Name.ToLower() == stringToCompare.ToLower()) != null;
        }
        private static bool NameIsInDietPlanTable(HumaneSocietyDataContext database, string stringToCompare)
        {
            return database.DietPlans.Distinct().SingleOrDefault(Plan => Plan.Name.ToLower() == stringToCompare.ToLower()) != null;
        }

        public static Client GetClient(string userName, string password)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var clientInformation = db.Clients.Distinct().Select(Client => Client).Where(Client => Client.UserName == userName && Client.Password == password);
            return clientInformation as Client;
        }
        public static IQueryable<Adoption> GetUserAdoptionStatus(Client client)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var pendingAdoptions = db.Adoptions.Where(Adoption => Adoption.ClientId == client.ClientId).Select(Adoption => Adoption);
            return pendingAdoptions;
        }

        public static Animal GetAnimalByID(int iD)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var specifiedAnimal = db.Animals.Where(Animal => Animal.AnimalId == iD).Select(Animal => Animal);
            return (Animal)specifiedAnimal;
        }

        public static void Adopt(Animal animal, Client client)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var joinedAnimalAndAdoptionTable = db.Adoptions.AsEnumerable().Distinct().Join(db.Animals.AsEnumerable(), Adoption => Adoption.AnimalId, Animal => Animal.AnimalId, (Adoption, Animal) => new { Adoption, Animal });
            var clientAnimal = joinedAnimalAndAdoptionTable.Select(a => a).Where(a => a.Animal.AnimalId == animal.AnimalId && a.Adoption.ClientId == client.ClientId);
            clientAnimal.Select(a => a.Animal.AdoptionStatus = "pending");
            clientAnimal.Select(a => a.Adoption.ApprovalStatus = "pending");
            clientAnimal.Select(a => a.Adoption.AdoptionFee = 75);
            db.SubmitChanges();
        }

        public static void RunEmployeeQueries(Employee employee, string input)
        {
            EmployeeToVoidFunction runQueries;

            switch (input)
            {
                case "create":
                    runQueries = AddUsernameAndPassword;
                    break;
                case "read":
                    runQueries = ReadEmployee;
                    break;
                case "update":
                    runQueries = UpdateEmployee;
                    break;
                case "delete":
                    runQueries = DeleteEmployee;
                    break;
                default:
                    throw new ApplicationException("Application error occured.");
            }
            runQueries(employee);
        }

        private static void ReadEmployee(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var displayData = db.Employees.Distinct().Single(read => read.EmployeeNumber == employee.EmployeeNumber);
            Console.WriteLine(displayData.FirstName);
            Console.WriteLine(displayData.LastName);
            Console.WriteLine(displayData.UserName);
            Console.WriteLine(displayData.Password);
            Console.WriteLine(displayData.Email);
            Console.WriteLine(displayData.Animals);
            Console.WriteLine(displayData.EmployeeId);
            Console.WriteLine(displayData.EmployeeNumber);
            displayData
        }

        private static void UpdateEmployee(Employee employee)
        {

        }

        private static void DeleteEmployee(Employee employee)
        {

        }

        public static IQueryable<Client> RetrieveClients()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var clients = db.Clients.Distinct().Select(Client => Client);
            return clients;
        }
        public static IQueryable<USState> GetStates()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var states = db.USStates.Distinct().Select(State => State);
            return states;
        }
        public static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var joinedClientAndAddressTable = db.Clients.AsEnumerable().Distinct().Join(db.Addresses.AsEnumerable().Distinct(), Client => Client.AddressId, Address => Address.AddressId, (Client, Address) => new { Client, Address });
            var clientAddress = joinedClientAndAddressTable.Where(a => a.Address.AddressId == a.Client.AddressId).Select(a => a.Address);

            Client newClient = new Client()
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = username,
                Password = password,
                Email = email,
                Address = (Address)clientAddress,
            };
            newClient.Address.AddressLine1 = streetAddress;
            newClient.Address.Zipcode = zipCode;
            newClient.Address.USStateId = stateId;
            db.Clients.InsertOnSubmit(newClient);
            db.SubmitChanges();
        }
    }
}
