using System;
using System.Collections.Generic;
using System.IO;
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
                animalToUpdate.Demeanor = updates[4].ToLower().Trim();
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
            if (updates.ContainsKey(9))
            {
                UpdateRoom(animalToUpdate, db, updates[9]);
                
            }
            db.SubmitChanges();
            return;
        }

        private static void UpdateRoom(Animal animalToUpdate, HumaneSocietyDataContext db, string userEnteredNumber)
        {
            int possibleRoomNumber;
            bool isInt = Int32.TryParse(userEnteredNumber, out possibleRoomNumber);
            Room roomToClear;
            Room roomToPopulate;
            if (isInt == true)
            {
                roomToClear = db.Rooms.SingleOrDefault(r => r.AnimalId == animalToUpdate.AnimalId);
                if (roomToClear != null)
                {
                    roomToClear.AnimalId = null;
                }
                roomToPopulate = db.Rooms.SingleOrDefault(r => r.RoomNumber == possibleRoomNumber);
                if (roomToPopulate != null)
                {
                    roomToPopulate.AnimalId = animalToUpdate.AnimalId;
                }
                else
                {
                    throw new Exception("Room number to move animal to was not found.");
                }

            }
            else
            {
                throw new Exception("Room number to move animal to was not a valid number");
            }
        }

        public static void RemoveAnimal(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalToDelete = db.Animals.SingleOrDefault(Animal => Animal.AnimalId == animal.AnimalId);
            if (animalToDelete != null)
            {
                db.Animals.DeleteOnSubmit(animalToDelete);
                db.SubmitChanges();
            }
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
            var selectedSpecy = db.Species.Distinct().SingleOrDefault(Specy => Specy.Name.ToLower() == speciesName.ToLower());
            return selectedSpecy;
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
            var selectedDietPlan = db.DietPlans.Distinct().Single(DietPlan => DietPlan.Name.ToLower() == dietPlanName.ToLower());
            return selectedDietPlan;
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
            return db.Employees.SingleOrDefault(user => userName == user.UserName) != null;
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
            var clientInformation = db.Clients.Distinct().SingleOrDefault(Client => Client.UserName == userName && Client.Password == password);
            return clientInformation;
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
            var specifiedAnimal = db.Animals.SingleOrDefault(Animal => Animal.AnimalId == iD);
            return specifiedAnimal;
        }

        public static void Adopt(Animal animal, Client client)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var joinedAnimalAndAdoptionTable = db.Adoptions.AsEnumerable().Distinct().Join(db.Animals.AsEnumerable(), Adoption => Adoption.AnimalId, Animal => Animal.AnimalId, (Adoption, Animal) => new { Adoption, Animal });
            var clientAnimal = joinedAnimalAndAdoptionTable.SingleOrDefault(a => a.Animal.AnimalId == animal.AnimalId && a.Adoption.ClientId == client.ClientId);
            clientAnimal.Animal.AdoptionStatus = "pending";
            clientAnimal.Adoption.ApprovalStatus = "pending";
            clientAnimal.Adoption.AdoptionFee = 75;
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
        }

        private static void UpdateEmployee(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var employeeToUpdate = db.Employees.Distinct().Single(user => user.EmployeeNumber == employee.EmployeeNumber);

            employeeToUpdate = employee;
            db.SubmitChanges();
        }

        private static void DeleteEmployee(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var employeeToDelete = db.Employees.SingleOrDefault(user => user.EmployeeNumber == employee.EmployeeNumber);
            if (employeeToDelete != null)
            {
                db.Employees.DeleteOnSubmit(employeeToDelete);
                db.SubmitChanges();
            }
        }

        public static IEnumerable<Client> RetrieveClients()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var currentClients = db.Clients.Select(Client => Client);
            return currentClients;
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
            Client newClient = new Client()
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = username,
                Password = password,
                Email = email
            };
            newClient.Address = new Address()
            {
                AddressLine1 = streetAddress,
                Zipcode = zipCode,
                USStateId = stateId
            };
            db.Clients.InsertOnSubmit(newClient);
            db.Addresses.InsertOnSubmit(newClient.Address);
            db.SubmitChanges();
        }
        public static void UpdateClient(Client client)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var clientToUpdate = db.Clients.Distinct().SingleOrDefault(Client => Client.ClientId == client.ClientId);
            clientToUpdate = client;
            db.SubmitChanges();
        }
        

        public static Room GetRoom(int animalId)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalRoom = db.Rooms.SingleOrDefault(r => r.AnimalId == animalId);
            return animalRoom;
        }

        public static bool CheckIfCSVFileValid(string file)
        {
            bool fileDoesExist = File.Exists(file);
            return fileDoesExist;
        }

        public static void ReadCSVFile(string file)
        {
            IEnumerable<string> stringOfCSV = File.ReadLines(file);
            var results = stringOfCSV.Select(s => s.Split(',').Skip(1));
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            foreach (string[] s in results)
            {
                
                bool rowIsValid = true;
                for (int i = 0; i < s.Length; i++)
                {
                    bool columnIsValid = ValidateFileInput(i, s[i], db);
                    if (columnIsValid == false)
                    {
                        rowIsValid = false;
                        Console.WriteLine($"Column containing {s[i]} was invalid. Row {i + 1} from file was skipped.");
                        break;
                    }
                }
                if (rowIsValid == true)
                {
                    Animal animal = new Animal();
                    for (int j = 0; j < s.Length; j++)
                    {
                        InsertColumnToNewAnimal(animal, db, j, s);
                    }
                    db.Animals.InsertOnSubmit(animal);
                }
            }
            db.SubmitChanges();
        }

        public static void InsertColumnToNewAnimal(Animal animal, HumaneSocietyDataContext db, int j, string[] s)
        {
            if (j == 0)
            {
                animal.Name = s[j];
            } 
            else if (j == 1)
            {
                animal.SpeciesId = Convert.ToInt32(s[j]);
            }
            else if (j == 2)
            {
                animal.Weight = Convert.ToInt32(s[j]);
            }
            else if (j == 3)
            {
                animal.Age = Convert.ToInt32(s[j]);
            }
            else if (j == 4)
            {
                animal.DietPlanId = Convert.ToInt32(s[j]);
            }
            else if (j == 5)
            {
                animal.Demeanor = s[j];
            }
            else if (j == 6)
            {
                animal.KidFriendly = Convert.ToBoolean(s[j]);
            }
            else if (j == 7)
            {
                animal.PetFriendly = Convert.ToBoolean(s[j]);
            }
            else  if (j == 8)
            {
                animal.Gender = s[j];
            }
            else if (j == 9)
            {
                if (s[j] == "not adopted")
                {
                    animal.AdoptionStatus = "available";
                }
                else
                {
                    animal.AdoptionStatus = s[j];
                }
            }
            else if (j == 10)
            {
                animal.EmployeeId = Convert.ToInt32(s[j]);
            }

        }

        private static bool ValidateFileInput(int i, string columnValue, HumaneSocietyDataContext db)
        {
            if (i == 1 || i == 2 || i == 3 || i == 4 || i == 10)
            {
                if (columnValue == null)
                {
                    return true;
                }
                else
                {
                    int result;
                    bool isNumber = Int32.TryParse(columnValue, out result);
                    if (isNumber == true && i == 1)
                    {
                        bool isValidSpecies = ValidateSpeciesInput(result, db);
                        return isValidSpecies;
                    }
                    else if (isNumber == true && i == 4)
                    {
                        bool isValidDiet = ValidateDietPlanInput(result, db);
                        return isValidDiet;
                    }
                    else if (isNumber == true && i == 10)
                    {
                        bool isValidEmployee = ValidateEmployeeInput(result, db);
                        return isValidEmployee;
                    }
                    else
                    {
                        return isNumber;
                    }
                }
            }
            else if (i == 6 || i == 7)
            {
                if (columnValue == null)
                {
                    return true;
                }
                else
                {
                    bool isBool = bool.TryParse(columnValue, out isBool);
                    return isBool;
                }
            }
            else
            {
                return true;
            }
        }

        private static bool ValidateSpeciesInput(int result, HumaneSocietyDataContext db)
        {
            return db.Species.SingleOrDefault(s => s.SpeciesId == result) != null;
        }

        private static bool ValidateDietPlanInput(int result, HumaneSocietyDataContext db)
        {
            return db.DietPlans.SingleOrDefault(d => d.DietPlanId == result) != null;
        }

        private static bool ValidateEmployeeInput(int result, HumaneSocietyDataContext db)
        {
            return db.Employees.SingleOrDefault(e => e.EmployeeId == result) != null;
        }
    }
}
