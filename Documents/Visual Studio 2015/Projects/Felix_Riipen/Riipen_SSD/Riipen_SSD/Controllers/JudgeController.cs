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
            var contestJudges = (from c in context.ContestJudges where c.JudgeUserId == UserID select c).ToList();
            List<Contest> contestList = new List<Contest>();
            foreach (var contestJudge in contestJudges)
            {
                contestList.Add(_unitOfWork.Contests.Get(contestJudge.ContestId));
            }

            return View(contestList);
        }


        public ActionResult contest(int contestID)
        {
            List<Team> teams = _unitOfWork.Contests.Get(contestID).Teams.ToList();
            List <TeamCriteriaScoreVM> teamCriteriaScoreVMList = new List<TeamCriteriaScoreVM>();
            double? YourScore = null;
            double? FinalScore = null;

            //get judge number for a contest
            int judgesNumber = _unitOfWork.ContestJudges.Find(cj => cj.ContestId == contestID).Count();
            int? judgeNotSubmit = null;
         
            foreach (var team in teams) {
                //get unsubmitted judge number for a team
                List<CriteriaScore> getSubmitJudges = context.CriteriaScores.Where(ci=>ci.ContestId==contestID && ci.TeamId == team.Id && (bool)ci.Submitted).ToList();
                judgeNotSubmit = judgesNumber - getSubmitJudges.Count();

                //get your score for a team
                string UserID = User.Identity.GetUserId();

                
                List<CriteriaScore> getYourCriteriaScoreForATeam = context.CriteriaScores.Where(ci => ci.ContestId == contestID && ci.TeamId == team.Id &&ci.Judge_ID==UserID).ToList();
                

                if (getYourCriteriaScoreForATeam.Count() != 0)
                {
                    getYourCriteriaScoreForATeam = getYourCriteriaScoreForATeam.Where(x => x.Score != null).ToList();

                    if (getYourCriteriaScoreForATeam.Count() != 0)
                    {
                        YourScore = Math.Round((double)getYourCriteriaScoreForATeam.Average(g => g.Score), 2);
                    }
                   
                  
                }
                else {
                    List<Criterion> getContestCriteria = _unitOfWork.Contests.Get(contestID).Criteria.ToList();

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

                teamCriteriaScoreVMList.Add(new TeamCriteriaScoreVM(team.Id, contestID,team.Name,YourScore,FinalScore,judgeNotSubmit));

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
            }

            singleJudgeCriteriaScoreVM.singleCriteriaScoreVMLlist = singleCriteriaScoreVMList;
            singleJudgeCriteriaScoreVM.Feedback = Feedback;

            ViewBag.TeamID = teamID;
            ViewBag.TeamName = TeamName;
            ViewBag.ContestName = _unitOfWork.Contests.Get(_unitOfWork.Teams.Get(teamID).ContestId).Name;
            return View(singleJudgeCriteriaScoreVM);

        }


        public ActionResult EditCriteriaScore(int teamID, int criteriaID)
        {
            string UserID = User.Identity.GetUserId();
            CriteriaScore criteriaScore = context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.CriteriaId == criteriaID && cs.Judge_ID == UserID).FirstOrDefault();
            Criterion criterion = _unitOfWork.Criteria.Get(criteriaID);
            CriteriaScoreVM criteriaScoreItem = new CriteriaScoreVM(criterion.Id, criterion.Name, criterion.Description, criteriaScore.Score, null,criteriaScore.Comment,null, false);
            ViewBag.TeamID = teamID;
            ViewBag.TeamName = _unitOfWork.Teams.Get(teamID).Name;
            ViewBag.ContestName = _unitOfWork.Contests.Get(_unitOfWork.Teams.Get(teamID).ContestId).Name;
            return View(criteriaScoreItem);

        }

        [HttpPost]
        public ActionResult EditCriteriaScore(int teamID, int criteriaID,string Comment, int Score, string Click) {

            string UserID = User.Identity.GetUserId();
            CriteriaScore criteriaScore = context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.CriteriaId == criteriaID && cs.Judge_ID == UserID).FirstOrDefault();
            criteriaScore.Score = Score;
            criteriaScore.Comment = Comment;

            if (Click == "Submit") {
                criteriaScore.Submitted = true;
            }
            context.SaveChanges();

            return RedirectToAction("Team", new { teamID= teamID });
        }

        

    }
}