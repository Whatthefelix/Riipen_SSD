using Riipen_SSD.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.DAL.Repositories.Concrete_Implementations
{
    public class CriterionRepository : Repository<Criterion>, ICriterionRepository
    {
        public CriterionRepository(Riipen_SSDEntities context) : base(context)
        {
        }
    }
}