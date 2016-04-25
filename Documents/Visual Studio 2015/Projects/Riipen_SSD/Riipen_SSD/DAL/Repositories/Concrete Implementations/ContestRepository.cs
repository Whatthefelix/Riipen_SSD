using Riipen_SSD.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Riipen_SSD.DAL.Repositories.Concrete_Implementations
{
    //public class ContestRepository : Repository<Contest>, IContestRepository
    //{
    //    public ContestRepository(Riipen_SSDEntities context) : base(context)
    //    {
    //    }

    //    public Riipen_SSDEntities Riipen_SSDEntities
    //    {
    //        get { return Context as Riipen_SSDEntities; }
    //    }

    //    public IEnumerable<Contest> GetAllForUser(string userID)
    //    {
    //        var user = Riipen_SSDEntities.AspNetUsers.FirstOrDefault(x => x.Id == userID);
    //        var contests = user.Teams.Select(x => x.Contest);

    //        return contests;
    //    }
    //}
}