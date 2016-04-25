using Riipen_SSD.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.DAL.Repositories.Concrete_Implementations
{
    public class ContestJudgeRepository : Repository<ContestJudge>, IContestJudgeRepository
    {
        public ContestJudgeRepository(Riipen_SSDEntities context) : base(context)
        {
        }

        public Riipen_SSDEntities Riipen_SSDEntities
        {
            get { return Context as Riipen_SSDEntities; }
        }
    }
}