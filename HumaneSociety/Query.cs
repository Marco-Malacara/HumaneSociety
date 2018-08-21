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
            var adoptionToUpdate = db.Adoptions.SingleOrDefault(a => adoption.AdoptionId == a.AdoptionId);
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
            var pendingAdoptions = db.Adoptions.Select(Animal => Animal).Where(Animal => Animal.ApprovalStatus == "pending");
            return pendingAdoptions;
        }

        public static IEnumerable<Animal> SearchForAnimalByMultipleTraits()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var search = db.Animals.Select(animal => animal).OrderBy(animal => animal.AnimalId);
            return search;
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
    }
}
