using Riipen_SSD.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.DAL.Repositories.Concrete_Implementations
{
    public class AspNetRoleRepository : Repository<AspNetRole>, IAspNetRoleRepository
    {
        public AspNetRoleRepository(SSD_RiipenEntities context) : base(context)
        {
        }

        public SSD_RiipenEntities SSD_RiipenEntities
        {
            get { return Context as SSD_RiipenEntities; }
        }
    }
}