using Riipen_SSD.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Riipen_SSD.DAL.Repositories.Concrete_Implementations
{
    public class ContestRepository : Repository<Contest>, IContestRepository
    {
        public ContestRepository(SSD_RiipenEntities context) : base(context)
        {
        }

        public SSD_RiipenEntities SSD_RiipenEntities
        {
            get { return Context as SSD_RiipenEntities; }
        }

        public IEnumerable<Contest> GetAllForUser(string userID)
        {
            var user = SSD_RiipenEntities.AspNetUsers.FirstOrDefault(x => x.Id == userID);
            var contests = user.Teams.Select(x => x.Contest);

            return contests;
        }
    }
}