using CsvHelper;
using Microsoft.AspNet.Identity;
using Riipen_SSD.AdminViewModels;
using Riipen_SSD.DAL;
using Riipen_SSD.ExtensionMethods;
using Riipen_SSD.Models;
using Riipen_SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Riipen_SSD.BusinessLogic;
using Microsoft.AspNet.Identity.EntityFramework;
using Riipen_SSD.ViewModels.AdminViewModels;

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
        public ActionResult Index(String searchAContest, String sortContests, int? page)
        {
           IEnumerable<AdminViewModels.IndexContestVM> adminContests = UnitOfWork.Contests.GetAll().Select(x => new AdminViewModels.IndexContestVM() { Name = x.Name, StartTime = x.StartTime.ToString(), Location = x.Location, Published = true, ContestID = x.Id });
            string searchStringValue = "";
            string sortStringValue = "Latests contests";

            // if search input is not null or empty
            if (!String.IsNullOrEmpty(searchAContest))
            {
                adminContests = adminContests.Where(a => a.Name.ToUpper().Contains(searchAContest.ToUpper()));

                searchStringValue = searchAContest;
            }


            if (!String.IsNullOrEmpty(sortContests))
            {
                if (sortContests == "Name")
                {
                    adminContests = adminContests.OrderBy(a => a.Name).ThenBy(a => a.StartTime);
                    sortStringValue = "Name";

                }
                else if (sortContests == "Location")
                {
                    adminContests = adminContests.OrderBy(a => a.Location).ThenBy(a => a.StartTime);
                    sortStringValue = "Location";
                }
            }
            else
            {
                adminContests = adminContests.OrderBy(a => a.StartTime);
            }

            ViewBag.SearchStringValue = searchStringValue;
            ViewBag.SortStringValue = sortStringValue;

            const int PAGE_SIZE = 10;
            int pageNumber = (page ?? 1);
            IEnumerable<AdminViewModels.IndexContestVM> newAdminContests = adminContests;
            newAdminContests = newAdminContests.ToPagedList(pageNumber, PAGE_SIZE);

            return View(newAdminContests);
        }

        [HttpGet]
        public ActionResult ContestDetails(int contestID)
        {
            var contest = UnitOfWork.Contests.Get(contestID);
            var judges = contest.ContestJudges.Select(x => new JudgeVM()
            {
                FirstName = x.AspNetUser.FirstName,
                LastName = x.AspNetUser.LastName,
                Email = x.AspNetUser.Email
            });
            var participants = new List<ParticipantVM>();
            foreach (var team in contest.Teams)
            {
                var participantsFromTeam = team.AspNetUsers.Select(x => new ParticipantVM() { Email = x.Email, FirstName = x.FirstName, LastName = x.LastName, TeamName = team.Name });
                participants.AddRange(participantsFromTeam);
            }

            var contestDetailVM = new ContestDetailsVM()
            {
                ContestID = contestID,
                Name = contest.Name,
                StartTime = contest.StartTime.ToString(),
                Location = contest.Location,
                Published = true,
                Participants = participants,
                Judges = judges,
            };

            ViewBag.ContestID = contestID;

            return View(contestDetailVM);
        }
        [HttpPost]
        public ActionResult SendEmails(int contestID)
        {
            //on "publish" click, get list of judges and participants for this contest
            var contest = UnitOfWork.Contests.Get(contestID);
            var judges = contest.ContestJudges.Select(x => new JudgeVM()
            {
                FirstName = x.AspNetUser.FirstName,
                LastName = x.AspNetUser.LastName,
                Email = x.AspNetUser.Email
            });
            var participants = new List<ParticipantVM>();
            foreach (var team in contest.Teams)
            {
                var participantsFromTeam = team.AspNetUsers.Select(x => new ParticipantVM() { Email = x.Email, FirstName = x.FirstName, TeamName = team.Name });
                participants.AddRange(participantsFromTeam);
            }

            var contestDetailVM = new ContestDetailsVM()
            {
                ContestID = contestID,
                Name = contest.Name,
                StartTime = contest.StartTime.ToString(),
                Location = contest.Location,
                Published = true,
                Participants = participants,
                Judges = judges,
            };
   


            foreach (var participant in contestDetailVM.Participants)
            {
                var getUser = UnitOfWork.Users.SingleOrDefault(x => x.Email == participant.Email);

                var getID = getUser.Id;
                var code = UserManager.GenerateEmailConfirmationTokenAsync(getID);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userID = getID, code = code }, protocol: Request.Url.Scheme);
                MailHelper mailer = new MailHelper();
                string subject = "Your Riipen account";
                string body = "";
                if (getUser.EmailConfirmed)
                {

                    body = "Please log in to view your contest: <a href=\"http:\\riipen.whatthefelix.com\" >Log in </a>";
                }
                else
                {
                    body = "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\"> Confirm </a>";
                }

                string response = mailer.EmailFromArvixe(new Message(participant.Email, subject, body));

            }



            return View();
        }

        [HttpGet]
        public ActionResult CreateContest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateContest(CreateContestVM contestVM)
        {
            // THIS SEEMS TO WORK BUT NEEDS SOME REFACTORING 
            var contest = new Contest()
            {
                StartTime = contestVM.StartTime,
                Location = contestVM.Location,
                Name = contestVM.ContestName,
                Published = false,
            };

            // for each participant, check if they have a user account - create one if they don't
            if (contestVM.Participants != null)
            {
                foreach (var participantVM in contestVM.Participants)
                {
                    var participantUser = UnitOfWork.Users.SingleOrDefault(x => x.Email == participantVM.Email);
                    if (participantUser == null)
                    {
                        var participantApplicationUser = new ApplicationUser { UserName = participantVM.Email, Email = participantVM.Email, FirstName = participantVM.FirstName, LastName = participantVM.LastName };
                        participantUser = AutoMapper.Mapper.Map<ApplicationUser, AspNetUser>(participantApplicationUser);
                        UnitOfWork.Users.Add(participantUser);
                    }
                    // for each participant, add them to a team - create the team if it doesn't exist already
                    var team = contest.Teams.FirstOrDefault(t => t.Name == participantVM.TeamName);
                    if (team == null)
                    {
                        team = new Team() { Name = participantVM.TeamName };
                        contest.Teams.Add(team);
                    }
                    team.AspNetUsers.Add(participantUser);

                }
            }

            // for each participant, check if they have a user account - create one if they don't - then add them to contest judges
            foreach (var judge in contestVM.Judges)
            {
                var judgeUser = UnitOfWork.Users.SingleOrDefault(x => x.Email == judge.Email);
                if (judgeUser == null)
                {
                    var judgeApplicationUser = new ApplicationUser { UserName = judge.Email, Email = judge.Email, FirstName = judge.FirstName, LastName = judge.LastName };
                    judgeUser = AutoMapper.Mapper.Map<ApplicationUser, AspNetUser>(judgeApplicationUser);
                    UnitOfWork.Users.Add(judgeUser);
                }
                contest.ContestJudges.Add(new ContestJudge { AspNetUser = judgeUser });
            }

            // add criteria
            contest.Criteria.AddRange(contestVM.Criteria.Select(x => new Criterion() { Description = x.Description, Name = x.Name }));

            contest = UnitOfWork.Contests.Add(contest);
            UnitOfWork.Complete();

            return View(contestVM);
        }

        // GET: EditContest
        [HttpGet]
        public ActionResult EditContest(int contestId)
        {
            var contest = UnitOfWork.Contests.Get(contestId);

            var editContestVM = new EditContestVM()
            {
                ContestID = contestId,
                ContestName = contest.Name,
                StartTime = contest.StartTime,
                Location = contest.Location,
                Criteria = contest.Criteria.Select(c => new CriteriaVM() { Id = c.Id, Name = c.Name, Description = c.Description }),
                Judges = contest.ContestJudges.Select(c => new JudgeVM() { Email = c.AspNetUser.Email, FirstName = c.AspNetUser.FirstName, LastName = c.AspNetUser.LastName }),
                Participants = contest.Teams.SelectMany(x => x.AspNetUsers.Select(y => new ParticipantVM() { Email = y.Email, FirstName = y.FirstName, LastName = y.LastName, TeamName = x.Name })),
            };

            return View(editContestVM);
        }

    // POST: EditContest
    [HttpPost]
        public ActionResult EditContest(EditContestVM editContestVM, HttpPostedFileBase file)
        {
            var contest = UnitOfWork.Contests.Get(editContestVM.ContestID);
            contest.Name = editContestVM.ContestName;
            contest.Location = editContestVM.Location;
            contest.StartTime = editContestVM.StartTime;

            editContestVM.Criteria.Select(c => new Criterion() { Id = c.Id });

            return View();
        }
  

        //
        public ActionResult contestScores(int contestID)
        {   //need to get TeamID, TeamName, FinalScore, 
            
            
            List<Team> teams = UnitOfWork.Contests.Get(contestID).Teams.ToList(); //get all the teams
            List<TeamCriteriaScoreVM> teamCriteriaScoreVMList = new List<TeamCriteriaScoreVM>(); //get all the criteriascoreVMs as list
            
            return View();
        }
        
    }
}