using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query 
    {
        public void UpdateAdoption(bool isApproved, Adoption adoption)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            switch (isApproved)
            {
                case true:
                    db.Adoptions.SingleOrDefault(a => adoption.AdoptionId == a.AdoptionId);

                    break;
                case false:
                    break;
            }
        }
    }
}
