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
using System.Threading.Tasks;

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
           IEnumerable<AdminViewModels.IndexContestVM> adminContests = UnitOfWork.Contests.GetAll().Select(x => new AdminViewModels.IndexContestVM() { Name = x.Name, StartTime = x.StartTime.ToString(), Location = x.Location, Published = x.Published, ContestID = x.Id });
            string searchStringValue = "";
            string sortStringValue = "Latest Contests";

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
        public ActionResult ContestDetails(int? contestID)
        {

   
            if(contestID != null)
            {
                var contest = UnitOfWork.Contests.Get(contestID.Value);
                if(contest == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var judges = contest.ContestJudges.Select(x => new JudgeVM()
                {
                    FirstName = x.AspNetUser.FirstName,
                    LastName = x.AspNetUser.LastName,
                    Email = x.AspNetUser.Email
                });
                var participants = new List<ContestDetailsParticipantVM>();
                foreach (var team in contest.Teams)
                {
                    var participantsFromTeam = team.AspNetUsers.Select(x => new ContestDetailsParticipantVM() { Email = x.Email, FirstName = x.FirstName, LastName = x.LastName, TeamName = team.Name, RiipenUrl = x.RiipenUrl });
                    participants.AddRange(participantsFromTeam);
                }

                var contestDetailVM = new ContestDetailsVM()
                {
                    ContestID = contestID.Value,
                    Name = contest.Name,
                    StartTime = contest.StartTime.ToString(),
                    Location = contest.Location,
                    Published = contest.Published,
                    Participants = participants,
                    Judges = judges,
                };

                ViewBag.ContestID = contestID;

                return View(contestDetailVM);
            }
            else
            {
                return RedirectToAction("Index", "admin");
            }

           
        }
        [HttpPost]
        public async Task<ActionResult> SendEmails(int? contestID)
        {
            
            if(contestID != null)
            {
                //on "publish" click, get list of judges and participants for this contest

                var contest = UnitOfWork.Contests.Get(contestID.Value);
                contest.Published = true;
                UnitOfWork.Complete();
                var judges = contest.ContestJudges.Select(x => new JudgeVM()
                {
                    FirstName = x.AspNetUser.FirstName,
                    LastName = x.AspNetUser.LastName,
                    Email = x.AspNetUser.Email
                });
                var participants = new List<ContestDetailsParticipantVM>();
                foreach (var team in contest.Teams)
                {
                    var participantsFromTeam = team.AspNetUsers.Select(x => new ContestDetailsParticipantVM() { Email = x.Email, FirstName = x.FirstName, TeamName = team.Name });
                    participants.AddRange(participantsFromTeam);
                }

                var contestDetailVM = new ContestDetailsVM()
                {
                    ContestID = contestID.Value,
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
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(getID);

                    //var code = UserManager.GeneratePasswordResetTokenAsync(getID);
                    var callbackUrl = Url.Action("SetPassword", "Account", new { userID = getID, code = code }, protocol: Request.Url.Scheme);
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


                
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }

        }

        [HttpGet]
        public ActionResult CreateContest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateContest(CreateContestVM contestVM)
        {
            if(!ModelState.IsValid){
                return View();
            }
            // THIS SEEMS TO WORK BUT NEEDS SOME REFACTORING 
            var contest = new Contest()
            {
                StartTime = contestVM.StartTime,
                Location = contestVM.Location,
                Name = contestVM.ContestName,
                Published = false,
                PubliclyViewable = false,
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
            if (contestVM.Judges != null)
            {
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
            }

            // add criteria
            contest.Criteria.AddRange(contestVM.Criteria.Select(x => new Criterion() { Description = x.Description, Name = x.Name }));

            contest = UnitOfWork.Contests.Add(contest);
            UnitOfWork.Complete();

            return RedirectToAction("ContestDetails", "Admin", new { contestID = contest.Id });
        }

        // GET: EditContest
        [HttpGet]
        public ActionResult EditContest(int? contestId)
        {
            if (contestId != null)
            {


                var contest = UnitOfWork.Contests.Get(contestId.Value);

                var editContestVM = new EditContestVM()
                {
                    ContestID = contestId.Value,
                    ContestName = contest.Name,
                    StartTime = contest.StartTime,
                    Location = contest.Location,
                    Criteria = contest.Criteria.Select(c => new CriteriaVM() { Id = c.Id, Name = c.Name, Description = c.Description }),
                    Judges = contest.ContestJudges.Select(c => new JudgeVM() { Email = c.AspNetUser.Email, FirstName = c.AspNetUser.FirstName, LastName = c.AspNetUser.LastName }),
                    Participants = contest.Teams.SelectMany(x => x.AspNetUsers.Select(y => new ParticipantVM() { Email = y.Email, FirstName = y.FirstName, LastName = y.LastName, TeamName = x.Name })),
                };

                return View(editContestVM);
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
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
        //public ActionResult ContestScores(int? contestID)
        //{   //need to get TeamID, TeamName, FinalScore, 
        //    if(contestID == null)
        //    {
        //        return RedirectToAction("index", "admin");
        //    }

        //    List<Team> teams = UnitOfWork.Contests.Get(contestID.Value).Teams.ToList(); //get all the teams
        //    List<TeamCriteriaScoreVM> teamCriteriaScoreVMList = new List<TeamCriteriaScoreVM>(); //get all the criteriascoreVMs as list




        //    //select winner and end contest


        //    return View();
        //}

        public ActionResult ContestScores(int? contestID, String searchATeam, String sortTeams, int? page)
        {
            if (contestID != null)
            {
                SSD_RiipenEntities context = new SSD_RiipenEntities();
                string searchStringValue = "";
                string sortStringValue = "Status";
                if (UnitOfWork.Contests.Get(contestID.Value) == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                //get the number of team in this contest
                List<Team> teams = UnitOfWork.Contests.Get(contestID.Value).Teams.ToList();
         
                if (!String.IsNullOrEmpty(searchATeam))
                {
                    teams = teams.Where(t => t.Name.ToUpper().Contains(searchATeam.ToUpper())).ToList();
                    searchStringValue = searchATeam;
                }

                List<TeamCriteriaScoreVM> teamCriteriaScoreVMList = new List<TeamCriteriaScoreVM>();
                List<Criterion> getContestCriteria = UnitOfWork.Contests.Get(contestID.Value).Criteria.ToList();

                double? YourScore = null;
                double? FinalScore = null;
                bool Submitted = false;


                //get judge number for a contest
                int judgesNumber = UnitOfWork.ContestJudges.Find(cj => cj.ContestId == contestID).Count();

                int? judgeNotSubmit = null;

                foreach (var team in teams)
                {
                    //get unsubmitted judge number for a team
                    //get the number of criteria for a contest               
                    List<CriteriaScore> getSubmitJudges = context.CriteriaScores.Where(ci => ci.ContestId == contestID && ci.TeamId == team.Id && (bool)ci.Submitted).GroupBy(x => x.Judge_ID).Select(x => x.FirstOrDefault()).ToList();

                    if (getContestCriteria.Count() != 0)
                    {
                        judgeNotSubmit = judgesNumber - getSubmitJudges.Count();
                    }
                    else
                    {
                        judgeNotSubmit = judgesNumber;
                    }

                    //get all the unsubmitted judge names
                    List<string> namesOfJudgeNotSubmitted = new List<string>();

                    //get all judges for a contest
                    var contestJudges = context.ContestJudges.Where(c => c.ContestId == team.ContestId).ToList();

                    //get the unsubmited 
                    List<ContestJudge> unsubmitedJudges = new List<ContestJudge>();

                    if (getSubmitJudges.Count() == 0)
                    {
                        unsubmitedJudges = contestJudges;
                    }
                    else
                    {
                        unsubmitedJudges = (from cj in contestJudges
                                            from g in getSubmitJudges
                                            where cj.JudgeUserId != g.Judge_ID
                                            select cj).ToList();
                    }

                    //get the unsubmitted name
                    foreach (var item in unsubmitedJudges)
                    {
                        var user = context.AspNetUsers.Find(item.JudgeUserId);
                        string name = user.FirstName + " " + user.LastName;
                        namesOfJudgeNotSubmitted.Add(name);
                    }


                    //get your score for a team
                    string UserID = User.Identity.GetUserId();
                    List<CriteriaScore> getYourCriteriaScoreForATeam = context.CriteriaScores.Where(ci => ci.ContestId == contestID && ci.TeamId == team.Id && ci.Judge_ID == UserID).ToList();

                    if (getYourCriteriaScoreForATeam.Count() != 0)
                    {
                        getYourCriteriaScoreForATeam = getYourCriteriaScoreForATeam.Where(x => x.Score != null).ToList();

                        if (getYourCriteriaScoreForATeam.Count() != 0)
                        {
                            YourScore = Math.Round((double)getYourCriteriaScoreForATeam.Average(g => g.Score), 2);

                            //check if has submitted
                            if (getYourCriteriaScoreForATeam.First().Submitted)
                            {
                                Submitted = true;
                            }
                        }
                    }
                    else {
                        foreach (var item in getContestCriteria)
                        {
                            CriteriaScore newCriteriaScore = new CriteriaScore();
                            newCriteriaScore.TeamId = team.Id;
                            newCriteriaScore.CriteriaId = item.Id;
                            newCriteriaScore.ContestId = contestID.Value;
                            newCriteriaScore.Judge_ID = User.Identity.GetUserId();
                            context.CriteriaScores.Add(newCriteriaScore);
                            context.SaveChanges();
                        }
                    }

                    //get final score for a team
                    if (judgeNotSubmit == 0)
                    {
                        List<CriteriaScore> getFinalCriteriaScoreForATeam = context.CriteriaScores.Where(ci => ci.ContestId == contestID && ci.TeamId == team.Id).ToList();
                        if (getFinalCriteriaScoreForATeam.Count() != 0)
                        {
                            FinalScore = Math.Round((double)getFinalCriteriaScoreForATeam.Average(g => g.Score), 2);
                        }
                    }

                    teamCriteriaScoreVMList.Add(new TeamCriteriaScoreVM(team.Id, contestID.Value, team.Name, YourScore, FinalScore, judgeNotSubmit, namesOfJudgeNotSubmitted, Submitted));
                    YourScore = null;
                    FinalScore = null;
                    Submitted = false;
                }

                if (sortTeams == "Name")
                {
                    teamCriteriaScoreVMList = teamCriteriaScoreVMList.OrderBy(t => t.TeamName).ToList();
                    sortStringValue = "Name";

                }
                else
                {
                    teamCriteriaScoreVMList = teamCriteriaScoreVMList.OrderBy(t => t.JudgeNotSubmitted).ThenBy(t => t.TeamName).ToList();
                }

                ViewBag.searchStringValue = searchStringValue;
                ViewBag.sortStringValue = sortStringValue;
                ViewBag.contestName = UnitOfWork.Contests.Get(contestID.Value).Name;
                ViewBag.contestId = contestID;


                const int PAGE_SIZE = 10;
                int pageNumber = (page ?? 1);
                IEnumerable<TeamCriteriaScoreVM> newTeamCriteriaScoreVMList = teamCriteriaScoreVMList;
                newTeamCriteriaScoreVMList = teamCriteriaScoreVMList.ToPagedList(pageNumber, PAGE_SIZE);

                return View(newTeamCriteriaScoreVMList);
            }
            else
            {
                return RedirectToAction("Index", "Judge");
            }

        }



        //pick a winner
        [HttpPost]
        public ActionResult PickWinner(int FirstId, int SecondId, int ThirdId)
        {
            Contest contest = UnitOfWork.Teams.Get(FirstId).Contest;
            contest.WinnerTeamId = FirstId;
            contest.SecondTeamId = SecondId;
            contest.ThirdTeamId = ThirdId;
            UnitOfWork.Complete();
            return RedirectToAction("Index", "Admin");
        }




    }
}