using CsvHelper;
using Microsoft.AspNet.Identity;
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
        public ActionResult CreateContest(ContestVM editContestVM, HttpPostedFileBase file)
        {
            // THIS SEEMS TO WORK BUT NEEDS MAJOR REFACTORING 
            var contest = new Contest()
            {
                StartTime = editContestVM.Date,
                Location = editContestVM.Location,
                Name = editContestVM.ContestName,
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
                        UserManager.AddToRoles(participantApplicationUser.Id, new string[] { "Participant" });
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
            foreach (var judge in editContestVM.Judges)
            {
                var judgeUser = UnitOfWork.Users.SingleOrDefault(x => x.Email == judge.Email);

                if (judgeUser == null)
                {
                    var judgeApplicationUser = new ApplicationUser { UserName = judge.Email, Email = judge.Email, FirstName = judge.FirstName, LastName = judge.LastName };
                    UserManager.Create(judgeApplicationUser, "Pa$$w0rd");
                    UserManager.AddToRoles(judgeApplicationUser.Id, new string[] { "Judge" });
                    judgeUser = AutoMapper.Mapper.Map<ApplicationUser, AspNetUser>(judgeApplicationUser);
                }
                judges.Add(new ContestJudge()
                {
                    JudgeUserId = judgeUser.Id,
                });
            }
            contest.Criteria.AddRange(editContestVM.Criteria.Select(x => new Criterion() { Description = x.Description, Name = x.Name }));

            UnitOfWork.Complete();

            contest.ContestJudges.AddRange(judges);

            UnitOfWork.Complete();

            UnitOfWork.Contests.Add(contest);
            UnitOfWork.Complete();

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