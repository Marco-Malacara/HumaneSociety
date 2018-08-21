﻿using System;
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
            var pendingAdoptions = db.Adoptions.Join(db.Animals.AsEnumerable(), Adoption => Adoption.AnimalId, Animal => Animal.AnimalId, (Adoption, Animal) => new { Adoption, Animal}).Select(a => a).Where(a => a.Animal.AdoptionStatus == "pending").Cast<Adoption>();
            return pendingAdoptions;
        }

        public static IEnumerable<AnimalShot> GetShots(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var shots = db.AnimalShots.Join(db.Animals.AsEnumerable(), AnimalShot => AnimalShot.AnimalId, Animal => Animal.AnimalId, (AnimalShot, Animal) => new { AnimalShot, Animal }).Select(a => a).Where(a => a.AnimalShot.AnimalId == a.Animal.AnimalId).Cast<AnimalShot>();
            return shots;
        }
    }
}
