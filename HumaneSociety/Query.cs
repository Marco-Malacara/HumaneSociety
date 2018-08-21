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
            var pendingAdoptions = db.Adoptions.Select(Animal => Animal).Where(Animal => Animal.ApprovalStatus.ToLower() == "pending");
            return pendingAdoptions;
        }

        public static void UpdateShot(string shotType, Animal animal)
        {
            //HumaneSocietyDataContext db = new HumaneSocietyDataContext();var animalToUpdate = db.AnimalShots.Join(Shot => db.Shots.Name, AnimalShot => AnimalShot.ShotId, Shot => Shot.ShotId, (AnimalShot, Shot) => new { AnimalShot, Shot });
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
    }
}
