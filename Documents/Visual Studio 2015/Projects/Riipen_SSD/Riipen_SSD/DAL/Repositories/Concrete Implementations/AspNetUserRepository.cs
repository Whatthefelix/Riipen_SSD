using Riipen_SSD.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.DAL.Repositories.Concrete_Implementations
{
    public class AspNetUserRepository : Repository<AspNetUser>, IAspNetUserRepository
    {
        public AspNetUserRepository(Riipen_SSDEntities context) : base(context)
        {
        }

        public Riipen_SSDEntities Riipen_SSDEntities
        {
            get { return Context as Riipen_SSDEntities; }
        }
    }

}