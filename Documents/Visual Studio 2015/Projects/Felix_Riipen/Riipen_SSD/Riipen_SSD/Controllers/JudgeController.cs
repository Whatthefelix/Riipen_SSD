using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Riipen_SSD.DAL;
using Microsoft.AspNet.Identity;
using Riipen_SSD.Models.ViewModels;
using Riipen_SSD.ViewModels;
using PagedList;
using System.Collections;

namespace Riipen_SSD.Controllers
{
    [Authorize(Roles ="Admin, Judge")]
    public class JudgeController : Controller
    {
        SSD_RiipenEntities context = new SSD_RiipenEntities();
        private IUnitOfWork _unitOfWork;
        public JudgeController()
        {
            _unitOfWork = new UnitOfWork(new SSD_RiipenEntities());

        }

        public ActionResult Index(String searchAContest, String sortContests, int? page)
        {
            string searchStringValue = "";
            string sortStringValue = "Latest contests";
            string UserID = User.Identity.GetUserId();
            
            //get the numbers of contest which the judge can judge
            var contestJudges = (from c in context.ContestJudges where c.JudgeUserId == UserID select c).ToList();
            contestJudges = contestJudges.OrderByDescending(cj=>cj.Contest.StartTime).ToList(); 
            List<Contest> contestList = new List<Contest>();

            foreach (var contestJudge in contestJudges)
            {
                // if search input is not null or empty
                if (!String.IsNullOrEmpty(searchAContest))
                {
                    if (contestJudge.Contest.Name.ToUpper().Contains(searchAContest.ToUpper()))
                    {
                        contestList.Add(_unitOfWork.Contests.Get(contestJudge.ContestId));
                    }

                   searchStringValue =searchAContest;
                }
                else
                {
                    contestList.Add(_unitOfWork.Contests.Get(contestJudge.ContestId));
                }                    
            }

            //get all the contests for the admin account
            if (User.IsInRole("Admin")){
                 contestList = _unitOfWork.Contests.GetAll().ToList();

                if (!String.IsNullOrEmpty(searchAContest))
                {
                   contestList = contestList.Where(c => c.Name.ToUpper().Contains(searchAContest.ToUpper())).ToList();
                }
                
            }
            
            if (!String.IsNullOrEmpty(sortContests))
            {
                if (sortContests == "Name")
                {
                    contestList = contestList.OrderBy(c => c.Name).ToList();
                    sortStringValue = "Name";

                } else if (sortContests=="Location")
                {
                    contestList = contestList.OrderBy(c => c.Location).ToList();
                    sortStringValue = "Location";
                }
            }

            ViewBag.SearchStringValue = searchStringValue;
            ViewBag.SortStringValue = sortStringValue;

            const int PAGE_SIZE = 10;
            int pageNumber = (page ?? 1);
            IEnumerable<Contest> newContestList = contestList ;
            newContestList = newContestList.ToPagedList(pageNumber, PAGE_SIZE);
            return View(newContestList);
        }


        public ActionResult Contest(int contestID, String searchATeam, String sortTeams, int? page)
        {

            string searchStringValue = "";
            string sortStringValue = "Status";
            
            //get the number of team in this contest
            List<Team> teams = _unitOfWork.Contests.Get(contestID).Teams.ToList();

            if (!String.IsNullOrEmpty(searchATeam))
            {
                teams = teams.Where(t => t.Name.ToUpper().Contains(searchATeam.ToUpper())).ToList();
                searchStringValue = searchATeam;
            }

            List<TeamCriteriaScoreVM> teamCriteriaScoreVMList = new List<TeamCriteriaScoreVM>();
            List<Criterion> getContestCriteria = _unitOfWork.Contests.Get(contestID).Criteria.ToList();

            double? YourScore = null;
            double? FinalScore = null;
            bool Submitted = false;

           
            //get judge number for a contest
            int judgesNumber = _unitOfWork.ContestJudges.Find(cj => cj.ContestId == contestID).Count();

            int? judgeNotSubmit = null;
         
            foreach (var team in teams) {
                //get unsubmitted judge number for a team
                //get the number of criteria for a contest               
                List<CriteriaScore> getSubmitJudges = context.CriteriaScores.Where(ci=>ci.ContestId==contestID && ci.TeamId == team.Id && (bool)ci.Submitted).GroupBy(x=>x.Judge_ID).Select(x=>x.FirstOrDefault()).ToList();

                if (getContestCriteria.Count()!=0)
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

                if (getSubmitJudges.Count()==0)
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
                foreach(var item in unsubmitedJudges)
                {
                    namesOfJudgeNotSubmitted.Add(context.AspNetUsers.Find(item.JudgeUserId).Email);
                }
                                                

                //get your score for a team
                string UserID = User.Identity.GetUserId();
                List<CriteriaScore> getYourCriteriaScoreForATeam = context.CriteriaScores.Where(ci => ci.ContestId == contestID && ci.TeamId == team.Id &&ci.Judge_ID==UserID).ToList();
                
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
                        newCriteriaScore.ContestId = contestID;
                        newCriteriaScore.Judge_ID = User.Identity.GetUserId();
                        context.CriteriaScores.Add(newCriteriaScore);
                        context.SaveChanges();                       
                    }                                      
                }

                //get final score for a team
                if (judgeNotSubmit == 0) {
                    List<CriteriaScore> getFinalCriteriaScoreForATeam = context.CriteriaScores.Where(ci => ci.ContestId == contestID && ci.TeamId == team.Id).ToList();
                    if (getFinalCriteriaScoreForATeam.Count() != 0) {
                        FinalScore = Math.Round((double)getFinalCriteriaScoreForATeam.Average(g => g.Score), 2);
                    }
                }

                teamCriteriaScoreVMList.Add(new TeamCriteriaScoreVM(team.Id, contestID, team.Name, YourScore, FinalScore, judgeNotSubmit,namesOfJudgeNotSubmitted,Submitted));
                YourScore = null;
                FinalScore = null;
                Submitted = false;
            }
           
            if (sortTeams == "Name")
            {
                teamCriteriaScoreVMList = teamCriteriaScoreVMList.OrderBy(t => t.TeamName).ToList();
                sortStringValue = "Name";

            }else
            {
                teamCriteriaScoreVMList = teamCriteriaScoreVMList.OrderBy(t => t.JudgeNotSubmitted).ThenBy(t=>t.TeamName).ToList();
            }

            ViewBag.searchStringValue = searchStringValue;
            ViewBag.sortStringValue = sortStringValue;
            ViewBag.contestName = _unitOfWork.Contests.Get(contestID).Name;
            ViewBag.contestId = contestID;


            const int PAGE_SIZE = 10;
            int pageNumber = (page ?? 1);
            IEnumerable<TeamCriteriaScoreVM> newTeamCriteriaScoreVMList = teamCriteriaScoreVMList;
            newTeamCriteriaScoreVMList = teamCriteriaScoreVMList.ToPagedList(pageNumber, PAGE_SIZE);

            return View(newTeamCriteriaScoreVMList);

        }


        public ActionResult Team(int teamID)
        {
            //get your all criteria Score mark for a team 
            string UserID = User.Identity.GetUserId();
            int contestID = _unitOfWork.Teams.Get(teamID).ContestId;
            string TeamName = _unitOfWork.Teams.Get(teamID).Name;

            List<SingleCriteriaScoreVM> singleCriteriaScoreVMList = new List<SingleCriteriaScoreVM>();
            SingleJudgeCriteriaScoreVM singleJudgeCriteriaScoreVM = new SingleJudgeCriteriaScoreVM();
            List<CriteriaScore> getYourAllCriteriaScore = context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.ContestId == contestID && cs.Judge_ID == UserID).ToList();
           

            foreach (var item in getYourAllCriteriaScore) {
                Criterion criteria = _unitOfWork.Criteria.Get(item.CriteriaId);
                string CriteriaName = criteria.Name;
                string Desciption = criteria.Description;
                double? Score = item.Score;
                string Comment = item.Comment;
                singleCriteriaScoreVMList.Add(new SingleCriteriaScoreVM(item.CriteriaId, CriteriaName, Desciption, Score, Comment));
            }

            String PublicFeedback = null;
            String PrivateFeedback = null;
            Feedback getFeedback = context.Feedbacks.Find(contestID, UserID, teamID);
            if (getFeedback != null) {

                PublicFeedback = getFeedback.PublicComment;
                PrivateFeedback = getFeedback.PrivateComment;
               
            }
            else
            {
                Feedback newFeedback = new Feedback();
                newFeedback.ContestId = contestID;
                newFeedback.TeamId = teamID;
                newFeedback.JudgeUserId = UserID;
                context.Feedbacks.Add(newFeedback);
                context.SaveChanges();
            }

            singleJudgeCriteriaScoreVM.singleCriteriaScoreVMLlist = singleCriteriaScoreVMList;
            singleJudgeCriteriaScoreVM.PublicFeedback = PublicFeedback;
            singleJudgeCriteriaScoreVM.PrivateFeedback = PrivateFeedback;

            ViewBag.TeamID = teamID;
            ViewBag.TeamName = TeamName;
            ViewBag.ContestID = contestID;
            ViewBag.ContestName = _unitOfWork.Contests.Get(_unitOfWork.Teams.Get(teamID).ContestId).Name;

            return View(singleJudgeCriteriaScoreVM);

        }

        [HttpPost]
        public ActionResult EditCriteriaScore(SingleJudgeCriteriaScoreVM singleJugeCriteriaScoreVM, int teamID, int contestID, string SubmitOrSave)
        {
            string UserID = User.Identity.GetUserId();
            Feedback getFeedback = context.Feedbacks.Find(contestID, UserID, teamID);

            if (getFeedback != null)
            {
                getFeedback.PublicComment = singleJugeCriteriaScoreVM.PublicFeedback;
                getFeedback.PrivateComment = singleJugeCriteriaScoreVM.PrivateFeedback;
            }

            //save criteriaScore            
            foreach(var item in singleJugeCriteriaScoreVM.singleCriteriaScoreVMLlist)
            {
                CriteriaScore getCriteriaScore = context.CriteriaScores.Where(cs => cs.Judge_ID == UserID
                && cs.TeamId == teamID && cs.CriteriaId == item.CriteriaID && cs.ContestId == contestID).FirstOrDefault();

                if (getCriteriaScore != null) {
                    getCriteriaScore.Score = (int)item.Score;
                    getCriteriaScore.Comment = item.Comment;
                    if (SubmitOrSave == "Submit")                 
                    {
                        getCriteriaScore.Submitted = true;
                    }
                }

                context.SaveChanges();

            }

            return RedirectToAction("Contest",new { contestID=contestID});

        }

    }
}