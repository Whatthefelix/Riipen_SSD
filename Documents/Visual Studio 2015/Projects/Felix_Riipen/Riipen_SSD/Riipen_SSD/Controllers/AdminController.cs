using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
                 //aa

            }
        }

        private IUnitOfWork _unitOfWork;
        public AdminController()
        {
            _unitOfWork = new UnitOfWork(new SSD_RiipenEntities());
        }
        // GET: Admin
        public ActionResult Index()
        {
           IEnumerable<AdminViewModels.IndexContestVM> adminContests = _unitOfWork.Contests.GetAll().Select(x => new AdminViewModels.IndexContestVM() { Name = x.Name, StartTime = x.StartTime.ToString(), Location = x.Location, Published = true, ContestID = x.Id });

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
                var newJudgeUser = _unitOfWork.Users.SingleOrDefault(x => x.Email == judge.Email);

                if (newJudgeUser == null)
                {
                    var user = new ApplicationUser { UserName = judge.Email, Email = judge.Email, FirstName = judge.FirstName, LastName = judge.LastName };
                    var result = UserManager.Create(user, "Pa$$w0rd");
                    judgeIds.Add(user.Id);
                }
            }

            newContest = _unitOfWork.Contests.Add(newContest);
            _unitOfWork.Complete();

            _unitOfWork.ContestJudges.AddRange(judgeIds.Select(x => new ContestJudge()
            {
                ContestId = newContest.Id,
                JudgeUserId = x
            }));
            _unitOfWork.Complete();

            return View();
        }
        public ActionResult ContestDetails(int contestID)
        {
            var contest = _unitOfWork.Contests.Get(contestID);
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
            var contest = _unitOfWork.Contests.Get(contestID); //get contest ID

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
        
    }
}