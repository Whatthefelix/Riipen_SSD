using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Riipen_SSD.DAL;
using Microsoft.AspNet.Identity;
using Riipen_SSD.Models.ViewModels;
using Riipen_SSD.ViewModels;

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

        public ActionResult Index()
        {
            string UserID = User.Identity.GetUserId();
            
            //get the numbers of contest which the judge can judge
            var contestJudges = (from c in context.ContestJudges where c.JudgeUserId == UserID select c).ToList();

            //if() 

            List<Contest> contestList = new List<Contest>();


            foreach (var contestJudge in contestJudges)
            {
                contestList.Add(_unitOfWork.Contests.Get(contestJudge.ContestId));
            }
                        if (User.IsInRole("Admin")){
                 contestList = _unitOfWork.Contests.GetAll().ToList();

                
            }



            return View(contestList);
        }


        public ActionResult contest(int contestID)
        {
            //get the number of team in this contest
            List<Team> teams = _unitOfWork.Contests.Get(contestID).Teams.ToList();
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
               
                List<CriteriaScore> getSubmitJudges = context.CriteriaScores.Where(ci=>ci.ContestId==contestID && ci.TeamId == team.Id && (bool)ci.Submitted).ToList();
                
                judgeNotSubmit = judgesNumber - (getSubmitJudges.Count())/ getContestCriteria.Count();

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

                teamCriteriaScoreVMList.Add(new TeamCriteriaScoreVM(team.Id, contestID,team.Name,YourScore,FinalScore,judgeNotSubmit,Submitted));
                YourScore = null;
                FinalScore = null;
                Submitted = false;
            }

            ViewBag.ContestName = _unitOfWork.Contests.Get(contestID).Name;
           return View(teamCriteriaScoreVMList);

        }


        public ActionResult team(int teamID)
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

            String Feedback = null;
            Feedback getFeedback = context.Feedbacks.Find(contestID, UserID, teamID);
            if (getFeedback != null) {
                Feedback = getFeedback.Comment;
                ViewBag.PubliclyViewable = getFeedback.PubliclyViewable;
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
            singleJudgeCriteriaScoreVM.Feedback = Feedback;

            ViewBag.TeamID = teamID;
            ViewBag.TeamName = TeamName;
            ViewBag.ContestID = contestID;
            ViewBag.ContestName = _unitOfWork.Contests.Get(_unitOfWork.Teams.Get(teamID).ContestId).Name;
            return View(singleJudgeCriteriaScoreVM);

        }

        [HttpPost]
        public ActionResult EditCriteriaScore(SingleJudgeCriteriaScoreVM singleJugeCriteriaScoreVM, string viewAvailability, int teamID, int contestID, string SubmitOrSave)
        {
            string UserID = User.Identity.GetUserId();
            Feedback getFeedback = context.Feedbacks.Find(contestID, UserID, teamID);

            if (getFeedback != null)
            {
                getFeedback.Comment = singleJugeCriteriaScoreVM.Feedback;
                if(viewAvailability=="public")
                getFeedback.PubliclyViewable = true;
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