using Riipen_SSD.DAL.Repositories.Interfaces;
using System;

namespace Riipen_SSD.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        IContestRepository Contests { get; }
        ICriteriaScoreRepository CriteriaScores { get; }
        ICriterionRepository Criteria { get; }
        IFeedbackRepository Feedback { get; }
        ITeamRepository Teams { get; }
        IAspNetUserRepository Users { get; }
        IAspNetRoleRepository Roles { get; }
        IContestJudgeRepository ContestJudges { get; }

        int Complete();
    }
}