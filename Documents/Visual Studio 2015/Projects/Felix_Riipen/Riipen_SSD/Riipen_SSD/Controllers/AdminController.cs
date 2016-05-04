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
        public ActionResult CreateContest()
        {
            return View();
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
                var participantsFromTeam = team.AspNetUsers.Select(x => new ParticipantVM() { Email = x.Email, Name = x.FirstName, TeamName = team.Name });
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
                var participantsFromTeam = team.AspNetUsers.Select(x => new ParticipantVM() { Email = x.Email, Name = x.FirstName, TeamName = team.Name });
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

            

            return View();
        }


        [HttpPost]
        public ActionResult CreateContest(ContestVM contestVM, HttpPostedFileBase file)
        {
            // THIS SEEMS TO WORK BUT NEEDS MAJOR REFACTORING 
            var contest = new Contest()
            {
                StartTime = contestVM.Date,
                Location = contestVM.Location,
                Name = contestVM.ContestName,
            };

            var participants = new List<AspNetUser>();
            using (StreamReader streamReader = new StreamReader(file.InputStream))
            {
                var csv = new CsvReader(streamReader);
                while (csv.Read())
                {
                    var name = csv.GetField<string>("Name");
                    var firstName = name.Split(' ')[0];
                    var lastName = name.Split(' ')[1];
                    var email = csv.GetField<string>("Email");
                    var teamName = csv.GetField<string>("Team Name");

                    var participant = UnitOfWork.Users.SingleOrDefault(x => x.Email == email);
                    if (participant == null)
                    {
                        var participantApplicationUser = new ApplicationUser { UserName = email, Email = email, FirstName = firstName, LastName = lastName, PhoneNumberConfirmed = true };
                        UserManager.Create(participantApplicationUser, "Pa$$w0rd");
                        participant = AutoMapper.Mapper.Map<ApplicationUser, AspNetUser>(participantApplicationUser);
                    }
                    participants.Add(participant);

                    var team = contest.Teams.FirstOrDefault(t => t.Name == teamName);
                    if (team == null)
                    {
                        team = new Team() { Name = teamName };
                        contest.Teams.Add(team);
                    }
                    team.AspNetUsers.Add(participant);
                }
            }

            UnitOfWork.Complete();

            var judges = new List<ContestJudge>();
            foreach (var judge in contestVM.Judges)
            {
                var judgeUser = UnitOfWork.Users.SingleOrDefault(x => x.Email == judge.Email);

                if (judgeUser == null)
                {
                    var judgeApplicationUser = new ApplicationUser { UserName = judge.Email, Email = judge.Email, FirstName = judge.FirstName, LastName = judge.LastName };
                    UserManager.Create(judgeApplicationUser, "Pa$$w0rd");
                    judgeUser = AutoMapper.Mapper.Map<ApplicationUser, AspNetUser>(judgeApplicationUser);
                }
                judges.Add(new ContestJudge()
                {
                    JudgeUserId = judgeUser.Id,
                });
            }
            contest.Criteria.AddRange(contestVM.Criteria.Select(x => new Criterion() { Description = x.Description, Name = x.Name }));
            contest.ContestJudges.AddRange(judges);
            contest = UnitOfWork.Contests.Add(contest);
            UnitOfWork.Complete();

            return View(contestVM);
        }

        // GET: EditContest
        [HttpGet]
        public ActionResult EditContest(int contestId)
        {
            var contest = UnitOfWork.Contests.Get(contestId);
            var contestVM = new ContestVM()
            {
                ContestName = contest.Name,
                Date = contest.StartTime,
                Location = contest.Location,
                Criteria = contest.Criteria.Select(c => new CriteriaVM() { Id = c.Id, Name = c.Name, Description = c.Description }),
                Judges = contest.ContestJudges.Select(c => new JudgeVM() { Email = c.AspNetUser.Email, FirstName = c.AspNetUser.FirstName, LastName = c.AspNetUser.LastName }),
            };

            return View(contestVM);
    }

    // POST: EditContest
    [HttpPost]
        public ActionResult EditContest(EditContestVM editContestVM, HttpPostedFileBase file)
        {
            var contest = UnitOfWork.Contests.Get(editContestVM.ContestID);
            contest.Name = editContestVM.ContestName;
            contest.Location = editContestVM.Location;
            contest.StartTime = editContestVM.Date;

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