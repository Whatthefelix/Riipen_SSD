using Riipen_SSD.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.DAL.Repositories.Concrete_Implementations
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        public TeamRepository(SSD_RiipenEntities context) : base(context)
        {
        }
    }
}