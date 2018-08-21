using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query 
    {

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
        
        public static IEnumerable<Adoption> GetPendingAdoptions()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var pendingAdoptions = db.Adoptions.Select(Animal => Animal).Where(Animal => Animal.ApprovalStatus.ToLower() == "pending");
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

        public static IEnumerable<Animal> SearchForAnimalByMultipleTraits()
        {
            
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
        //public static IEnumerable<Specy> GetSpecies()
        //{
        //    HumaneSocietyDataContext db = new HumaneSocietyDataContext();
        //    var joinedAnimalAndSpecy = db.Animals.AsEnumerable().Join(db.Species.AsEnumerable(), Animal => Animal.SpeciesId, Specy => Specy.SpeciesId, (Animal, Specy) => new { Animal, Specy });
        //    string speciesName = UserInterface.GetStringData("the animal's", "species");
        //    if (NameIsInSpecies(db, speciesName))
        //    {

        //    }

        //    var selectedSpecy = joinedAnimalAndSpecy.Select(Animal => Animal.Specy);
        //    return selectedSpecy;
        //}
        private static bool NameIsInSpecies (HumaneSocietyDataContext database ,string stringToCompare )
        {
            return database.Species.SingleOrDefault(Specy => Specy.Name.ToLower() == stringToCompare.ToLower()) != null;
        }

    }
}
