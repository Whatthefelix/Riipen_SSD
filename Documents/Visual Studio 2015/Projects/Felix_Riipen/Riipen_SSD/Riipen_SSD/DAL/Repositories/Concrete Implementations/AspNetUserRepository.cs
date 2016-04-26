using Riipen_SSD.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.DAL.Repositories.Concrete_Implementations
{
    public class AspNetUserRepository : Repository<AspNetUser>, IAspNetUserRepository
    {
        public AspNetUserRepository(SSD_RiipenEntities context) : base(context)
        {
        }

        public SSD_RiipenEntities SSD_RiipenEntities
        {
            get { return Context as SSD_RiipenEntities; }
        }

        public bool CheckIfUserExists(string email)
        {
            return SSD_RiipenEntities.AspNetUsers.Any(x => x.Email == email);
        }
    }
    
}