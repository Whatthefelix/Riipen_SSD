using Microsoft.AspNet.Identity;
using Riipen_SSD.DAL;
using Riipen_SSD.Models;
using Riipen_SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Riipen_SSD.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationUserManager UserManager;
        private IUnitOfWork UnitOfWork;

        public AdminController(ApplicationUserManager userManager, IUnitOfWork unitOfWork)
        {
            UserManager = userManager;
            UnitOfWork = unitOfWork;
        }
        // GET: Admin
        public ActionResult Index()
        {
           IEnumerable<AdminViewModels.IndexContestVM> adminContests = UnitOfWork.Contests.GetAll().Select(x => new AdminViewModels.IndexContestVM() { Name = x.Name, StartTime = x.StartTime.ToString(), Location = x.Location, Published = true, ContestID = x.Id });

            return View(adminContests);
        }
        [HttpGet]
        public ActionResult CreateContest()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateContest(ContestVM contest)
        {
            ICollection<Criterion> criteriaList = contest.Criteria.Select(x => new Criterion() { Description = x.Description, Name = x.Name }).ToList();

            var newContest = new Contest()
            {
                Criteria = criteriaList,
                StartTime = contest.Date,
                Location = contest.Location,
                Name = contest.ContestName,
            };

            var judgeIds = new List<String>();
            foreach (var judge in contest.Judges)
            {
                var judgeApplicationUser = UserManager.FindByEmail(judge.Email);

                if (judgeApplicationUser == null)
                {
                    judgeApplicationUser = new ApplicationUser { UserName = judge.Email, Email = judge.Email, FirstName = judge.FirstName, LastName = judge.LastName };
                    var result = UserManager.Create(judgeApplicationUser, "Pa$$w0rd");
                }
                judgeIds.Add(judgeApplicationUser.Id);
            }

            newContest = UnitOfWork.Contests.Add(newContest);
            UnitOfWork.Complete();

            UnitOfWork.ContestJudges.AddRange(judgeIds.Select(x => new ContestJudge()
            {
                ContestId = newContest.Id,
                JudgeUserId = x
            }));

            UnitOfWork.Complete();

            return View();
        }
        public ActionResult ContestDetails(int contestID)
        {
            var contest = UnitOfWork.Contests.Get(contestID);
            var judges = contest.ContestJudges.Select(x => new AdminViewModels.JudgeVM()
            {
                FirstName = x.AspNetUser.FirstName,
                LastName = x.AspNetUser.LastName,
                Email = x.AspNetUser.Email
            });
            var participants = new List<AdminViewModels.ParticipantVM>();
            foreach (var team in contest.Teams)
            {
                var participantsFromTeam = team.AspNetUsers.Select(x => new AdminViewModels.ParticipantVM() { Email = x.Email, Name = x.FirstName, TeamName = team.Name });
                participants.AddRange(participantsFromTeam);
            }

            var contestDetailVM = new AdminViewModels.ContestDetailsVM()
            {
                ContestID = contestID,
                Name = contest.Name,
                StartTime = contest.StartTime.ToString(),
                Location = contest.Location,
                Published = true,
                Participants = participants,
                Judges = judges,
            };

            return View(contestDetailVM);
        }
        public ActionResult EditContest(int contestID)
        {
            var contest = UnitOfWork.Contests.Get(contestID); //get contest ID

            var judges = contest.ContestJudges.Select(x => new AdminViewModels.JudgeVM() 
            {
                FirstName = x.AspNetUser.FirstName,
                LastName = x.AspNetUser.LastName,
                Email = x.AspNetUser.Email
            });
            var criteria = contest.Criteria.Select(x => new AdminViewModels.CriteriaVM()
            {
                //criteria name
                Description = x.Description,
                Name = x.Name,
            
            });

            var editContestVM = new AdminViewModels.EditContestVM()
            {   
                
                ContestID = contestID,
                Judges = judges,
                Criteria = criteria,
                ContestName = contest.Name,
                Date = contest.StartTime,
                Location = contest.Location,
            };           

            return View(editContestVM);
        }
        [HttpPost]
        public ActionResult Editcontest(AdminViewModels.EditContestVM edit, int contestID)
        {
            return View();
        }
        
        public ActionResult contestScores(int contestID)
        {   //need to get TeamID, TeamName, FinalScore, 
            double? FinalScore = null;
            
            List<Team> teams = UnitOfWork.Contests.Get(contestID).Teams.ToList(); //get all the teams
            List<TeamCriteriaScoreVM> teamCriteriaScoreVMList = new List<TeamCriteriaScoreVM>(); //get all the criteriascoreVMs as list
            
            return View();
        }
    }
}