using Riipen_SSD.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.DAL.Repositories.Concrete_Implementations
{
    public class CriteriaScoreRepository : Repository<CriteriaScore>, ICriteriaScoreRepository
    {
        public CriteriaScoreRepository(SSD_RiipenEntities context) : base(context)
        {
        }
    }
}