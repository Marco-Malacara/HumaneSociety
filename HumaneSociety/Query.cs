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
            var animalToUpdate = db.Animals.Single(Animal => Animal.AnimalId == adoption.AnimalId);
            var shelterToUpdate = db.Shelters.Select(Shelter => Shelter);
            switch (isApproved)
            {
                case true:
                    adoptionToUpdate.ApprovalStatus = "approved";
                    animalToUpdate.AdoptionStatus = "adopted";
                    adoptionToUpdate.AdoptionFee = 0;
                    ((Shelter)shelterToUpdate).Money += 75;
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

        public static void AdministerShot(string shotName, Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            bool doesShotExist = CheckIfShotExists(shotName, db);
            if (doesShotExist)
            {
                int shotId = GetShotId(shotName, db);
                bool needsShot = CheckIfAnimalNeedsShot(shotId, db);
                if (needsShot)
                {
                    AnimalShot animalShot = new AnimalShot();
                    animalShot.AnimalId = animal.AnimalId;
                    animalShot.ShotId = shotId;
                    db.AnimalShots.InsertOnSubmit(animalShot);
                    db.SubmitChanges();
                }
                else
                {
                    Console.WriteLine("The animal already has this shot.");
                }
            }
            else
            {
                Console.WriteLine("The shot you are trying to administer does not exist. Would you like to create it?");
                bool createNewShot = (bool)UserInterface.GetBitData();
                if (createNewShot)
                {
                    CreateShot(shotName, db);
                    AdministerShot(shotName, animal);
                    return;
                }
            }

        }

        private static bool CheckIfShotExists(string shotName, HumaneSocietyDataContext db)
        {
            return (db.Shots.SingleOrDefault(s => s.Name == shotName.ToLower().Trim()) != null);
            
        }

        private static int GetShotId(string shotName, HumaneSocietyDataContext db)
        {
            return db.Shots.Single(s => shotName.ToLower().Trim() == s.Name).ShotId;
        }

        private static bool CheckIfAnimalNeedsShot(int shotId, HumaneSocietyDataContext db)
        {
            return (db.AnimalShots.SingleOrDefault(s => s.ShotId == shotId) == null);
        }

        public static void CreateShot(string shotName, HumaneSocietyDataContext db)
        {
            Shot newShot = new Shot();
            newShot.Name = shotName.Trim().ToLower();
            db.Shots.InsertOnSubmit(newShot);
            db.SubmitChanges();
        }

        public static IEnumerable<AnimalShot> GetShots(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var shots = db.AnimalShots.Where(AnimalShot => AnimalShot.AnimalId == animal.AnimalId);
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
                int number;
                bool isNumber = Int32.TryParse(searchParameters[3], out number);
                if (isNumber == true)
                {
                    animals = (from animal in animals where animal.Age == number select animal);
                }
                else
                {
                    Console.WriteLine("Could not search by age because a valid number was not entered.");
                }
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
                    animals = (from animal in animals where animal.KidFriendly == parameter select animal);
                }
                else if (searchParameters[5].ToLower().Trim() == "false" || searchParameters[5].ToLower().Trim() == "no" || searchParameters[5] == "0")
                {
                    parameter = false;
                    animals = (from animal in animals where animal.KidFriendly == parameter select animal);
                }
                else
                {
                    Console.WriteLine("Could not search by Kid Friendly value because the value entered was not valid.");
                }
                
            }
            if (searchParameters.ContainsKey(6))
            {
                bool parameter = false;
                if (searchParameters[6].ToLower().Trim() == "true" || searchParameters[6].ToLower().Trim() == "yes" || searchParameters[6] == "1")
                {
                    parameter = true;
                    animals = (from animal in animals where animal.PetFriendly == parameter select animal);
                }
                else if (searchParameters[6].ToLower().Trim() == "false" || searchParameters[6].ToLower().Trim() == "no" || searchParameters[6] == "0")
                {
                    parameter = false;
                    animals = (from animal in animals where animal.PetFriendly == parameter select animal);
                }
                else
                {
                    Console.WriteLine("Could not search by Pet Friendly value because the value entered was not valid.");
                }
                
            }
            if (searchParameters.ContainsKey(7))
            {
                int number;
                bool isNumber = Int32.TryParse(searchParameters[7], out number);
                if (isNumber == true)
                {
                    animals = (from animal in animals where animal.Weight == number select animal);
                }
                else
                {
                    Console.WriteLine("Could not search by weight because a valid number was not entered.");
                }
            }
            if (searchParameters.ContainsKey(8))
            {
                int number;
                bool isNumber = Int32.TryParse(searchParameters[8], out number);
                if (isNumber == true)
                {
                    animals = (from animal in animals where animal.AnimalId == number select animal);
                }
                else
                {
                    Console.WriteLine("Could not search by ID because a valid number was not entered.");
                }
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
                    CreateNewSpecies(updates[1].ToLower().Trim(), db);
                    animalToUpdate.SpeciesId = db.Species.Single(s => s.Name == updates[1].ToLower().Trim()).SpeciesId;
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
                    Console.WriteLine("Age was not updated because a number was not entered.");
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
                    Console.WriteLine("Kid Friendly value was not updated because a valid yes/no/true/false/1/0 value was not entered.");
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
                    Console.WriteLine("Pet Friendly value was not updated because a valid yes/no/true/false/1/0 value was not entered.");
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
                    Console.WriteLine("Weight could not be updated because a valid number was not entered.");
                }
            }
            if (updates.ContainsKey(8))
            {
                Console.WriteLine("Id was not updated because id cannot be changed.");
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
                    if (roomToPopulate.AnimalId == null)
                    {
                        roomToPopulate.AnimalId = animalToUpdate.AnimalId;
                    }
                    else
                    {
                        Console.WriteLine("The room was not updated because the room is already taken.");
                    }
                }
                else
                {
                    Console.WriteLine("Room number to move animal to was not found.");
                }

            }
            else
            {
                Console.WriteLine("Room number entered was not a number.");
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
                CreateNewSpecies(speciesName, db);
            }
            var selectedSpecy = db.Species.Distinct().SingleOrDefault(Specy => Specy.Name.ToLower() == speciesName.ToLower());
            return selectedSpecy;
        }

        private static void CreateNewSpecies(string speciesName, HumaneSocietyDataContext db)
        {
            db.Species.InsertOnSubmit(new Specy() { Name = speciesName });
            db.SubmitChanges();
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

        public static void AddEmployee(Employee employee)
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
                    runQueries = AddEmployee;
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

        public static void DisplayAnimalInfo(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalToDisplay = db.Animals.Distinct().Single(a => a.AnimalId == animal.AnimalId);
            Room animalRoom = GetRoom(animalToDisplay.AnimalId, db);
            List<string> info = new List<string>() { "ID: " + animalToDisplay.AnimalId, animalToDisplay.Name, animalToDisplay.Age + " years old", "Demeanor: " + animalToDisplay.Demeanor, "Kid friendly: " + UserInterface.BoolToYesNo(animalToDisplay.KidFriendly), "Pet friendly: " + UserInterface.BoolToYesNo(animalToDisplay.PetFriendly), "Location: " + animalRoom.RoomId, "Weight: " + animalToDisplay.Weight.ToString(), "Food amount in cups: " + animalToDisplay.DietPlan.FoodAmountInCups, "Food type: " + animalToDisplay.DietPlan.FoodType };
            UserInterface.DisplayUserOptions(info);
            Console.ReadLine();

        }

        public static IEnumerable<Employee> GetEmployees()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var employees = db.Employees.Distinct().AsEnumerable().Select(Employee => Employee);
            return employees;
        }

        public static Employee GetEmployeeById (int id)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var employee = db.Employees.Distinct().SingleOrDefault(Employee => Employee.EmployeeId == id);
            return employee;
        }

        private static void ReadEmployee(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Console.WriteLine("Firstname: " + employee.FirstName);
            Console.WriteLine("Lastname: " + employee.LastName);
            Console.WriteLine("Username: " + employee.UserName);
            Console.WriteLine("Password: " + employee.Password);
            Console.WriteLine("Email: " + employee.Email);
            Console.WriteLine("Animals: " + employee.Animals);
            Console.WriteLine("Employee I.D. " + employee.EmployeeId);
            Console.WriteLine("Employee Number: " + employee.EmployeeNumber);
            Console.WriteLine("Press [ENTER] to continue.");
            Console.ReadLine();
        }

        private static void UpdateEmployee(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var employeeToUpdate = db.Employees.Distinct().Single(user => user.EmployeeId == employee.EmployeeId);

            employeeToUpdate = employee;
            db.SubmitChanges();
        }

        private static void DeleteEmployee(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var employeeToDelete = db.Employees.SingleOrDefault(user => user.EmployeeId == employee.EmployeeId);
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
        

        public static Room GetRoom(int animalId, HumaneSocietyDataContext db)
        {
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
            var results = stringOfCSV.Select(s => s.Split(',').Skip(1).ToList());
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            foreach (List<string> s in results)
            {
                
                bool rowIsValid = true;
                for (int i = 0; i < s.Count; i++)
                {
                    bool columnIsValid = ValidateFileInput(i, s[i].Trim(), db);
                    if (columnIsValid == false)
                    {
                        rowIsValid = false;
                        Console.WriteLine($"Column {i + 2 } containing {s[i]} was invalid. The row with this column was skipped.");
                        break;
                    }
                }
                if (rowIsValid == true)
                {
                    Animal animal = new Animal();
                    for (int j = 0; j < s.Count; j++)
                    {
                        InsertColumnToNewAnimal(animal, db, j, s);
                    }
                    db.Animals.InsertOnSubmit(animal);
                }
            }
            db.SubmitChanges();
        }

        public static void InsertColumnToNewAnimal(Animal animal, HumaneSocietyDataContext db, int j, List<string> s)
        {
            if (s[j].Trim().Replace("\"", "") == "null")
            {
                //We don't need to insert null to the database, so nothing will happen in this method if the value in the file is null.
            }
            else
            {
                switch (j)
                {
                    case 0:
                        animal.Name = s[j].Trim().Replace("\"", "");
                        break;
                    case 1:
                        animal.SpeciesId = Convert.ToInt32(s[j]);
                        break;
                    case 2:
                        animal.Weight = Convert.ToInt32(s[j]);
                        break;
                    case 3:
                        animal.Age = Convert.ToInt32(s[j]);
                        break;
                    case 4:
                        animal.DietPlanId = Convert.ToInt32(s[j]);
                        break;
                    case 6:
                        animal.Demeanor = s[j].Trim().Replace("\"", "");
                        break;
                    case 7:
                        if (s[j].Trim() == "1")
                        {
                            animal.KidFriendly = true;
                        }
                        else if (s[j].Trim() == "0")
                        {
                            animal.KidFriendly = false;
                        }
                        break;
                    case 8:
                        switch (s[j].Trim())
                        {
                            case "1":
                                animal.PetFriendly = true;
                                break;
                            case "0":
                                animal.PetFriendly = false;
                                break;
                        }
                        break;
                    case 9:
                        animal.Gender = s[j].Trim().Replace("\"", "");
                        break;
                    case 10:
                        switch (s[j].Trim().Replace("\"", ""))
                        {
                            case "not adopted":
                                animal.AdoptionStatus = "available";
                                break;
                            default:
                                animal.AdoptionStatus = s[j].Trim().Replace("\"", "");
                                break;

                        }
                        break;
                    case 11:
                        animal.EmployeeId = Convert.ToInt32(s[j]);
                        break;
                }
            }
        }

        private static bool ValidateFileInput(int i, string columnValue, HumaneSocietyDataContext db)
        {
            if (i == 1 || i == 2 || i == 3 || i == 4 || i == 11)
            {
                if (columnValue == "null")
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
                    else if (isNumber == true && i == 11)
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
            else if (i == 7 || i == 8)
            {
                if (columnValue == "null")
                {
                    return true;
                }
                else
                {
                    if (columnValue == "1" || columnValue == "0")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
