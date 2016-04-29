using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Riipen_SSD.DAL;
using Riipen_SSD.ViewModels.ParticipantViewModels;
using Riipen_SSD.ViewModels;

namespace Riipen_SSD.Controllers
{
    public class ParticipantController : Controller
    {
        SSD_RiipenEntities context = new SSD_RiipenEntities();
        private IUnitOfWork _unitOfWork;

        public ParticipantController()
        {
            _unitOfWork = new UnitOfWork(new SSD_RiipenEntities());

        }



        public ActionResult Index()
        {
            string UserID = User.Identity.GetUserId();
            List<ParticipantContestVM> participantVMList = new List<ParticipantContestVM>();
            var teamList = context.AspNetUsers.Find(UserID).Teams.ToList();
            foreach (var item in teamList)
            {

                var getContest = context.Contests.Where(c => c.Id == item.ContestId).FirstOrDefault();
                participantVMList.Add(new ParticipantContestVM(item.Id, getContest.Id, getContest.Name, getContest.StartTime, getContest.Location));

            }

            return View(participantVMList);
        }

        public ActionResult Contests(int contestID)
        {
            List<ContestTeamVM> ContestTeamVMList = new List<ContestTeamVM>();

            //get all criteria for one contset
            List<Criterion> getContestCriteria = _unitOfWork.Contests.Get(contestID).Criteria.ToList();

            //Get all teams in one contest
            var teams = context.Teams.Where(t => t.ContestId == contestID).ToList();

            //get judge number for one contest
            int judgesNumber = _unitOfWork.ContestJudges.Find(cj => cj.ContestId == contestID).Count();

            double? CurrentScore = null;
            int? judgeNotSubmit = null;

            foreach (var team in teams)
            {
                //get current score for a team
                List<CriteriaScore> getAllAvailableScoreForATeam = context.CriteriaScores.Where(cs => cs.ContestId == contestID && cs.TeamId == team.Id && cs.Submitted).ToList();
               

                if (getAllAvailableScoreForATeam.Count() != 0)
                {
                    CurrentScore = Math.Round((double)getAllAvailableScoreForATeam.Average(x => x.Score), 2);
                }

                //get number of judges not submitted
                List<CriteriaScore> getSubmitJudges = context.CriteriaScores.Where(ci => ci.ContestId == contestID && ci.TeamId == team.Id && ci.Submitted).ToList();
                judgeNotSubmit = judgesNumber - (getSubmitJudges.Count()) / getContestCriteria.Count();

                ContestTeamVMList.Add(new ContestTeamVM(team.Id, team.Name,CurrentScore,judgeNotSubmit));

            }

            ViewBag.ContestID = contestID;

            ViewBag.ContestName = context.Contests.Find(contestID).Name;

            return View(ContestTeamVMList);

        }

        public ActionResult TeamScores(int teamID, int contestID)
        {
            TeamCriteriaVMList teamCriteriaVMListComplete = new TeamCriteriaVMList();

            List<TeamCriteriaVM> teamCriterVMlist = new List<TeamCriteriaVM>();

            double? CurrentScore = null;

            //get all criteria for a team 
            List<Criterion> getContestCriteria = _unitOfWork.Contests.Get(contestID).Criteria.ToList();

            //get all submitted criteria scores for one team
            List<CriteriaScore> getAllCriteriaScoreForOneTeam = context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.ContestId == contestID&&(bool)cs.Submitted).ToList();

            //get the current score (overall) of all criteria for a team in a contest
            if (getAllCriteriaScoreForOneTeam.Count() != 0) {
                CurrentScore = getAllCriteriaScoreForOneTeam.Average(x => x.Score);
            }

            //get each score for each criteria 
            foreach (var item in getContestCriteria) {
                //ge all scores for one criteria
                var getAllScoreForOneCriteria = getAllCriteriaScoreForOneTeam.Where(x => x.CriteriaId == item.Id).ToList();

                double? getAverageScoreForEachCriteria = null;

                if (getAllScoreForOneCriteria.Count() != 0) {
                    getAverageScoreForEachCriteria = Math.Round((double)getAllScoreForOneCriteria.Average(x => x.Score), 2);
                }

                teamCriterVMlist.Add(new TeamCriteriaVM(item.Id,item.Name,item.Description,getAverageScoreForEachCriteria));                
            }

            //get the feedbacks for a team in contest
            List<TeamFeedbackVM> teamFeedbackVMList = new List<TeamFeedbackVM>();
            var getAllFeedbacskForATeamInAContest = context.Feedbacks.Where(f => f.ContestId == contestID && f.TeamId == teamID && f.PubliclyViewable).ToList();

            //only display feedbacks when it is public and the judge has submitted the score
            foreach(var item in getAllFeedbacskForATeamInAContest)
            {
                //check if this judge has submitted his score
                if (context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.Judge_ID == item.JudgeUserId && cs.Submitted).ToList().Count() > 0) {
                    
                    //check if this judge write comment or not
                    if(item.Comment!=null && item.Comment != "")
                    { //get the judge name for this feedback 
                        string JudgeName = context.AspNetUsers.Find(item.JudgeUserId).UserName;
                        string Feedback = item.Comment;
                        teamFeedbackVMList.Add(new TeamFeedbackVM(JudgeName, Feedback));
                    }
                   
                };
            }
          
            teamCriteriaVMListComplete.teamCriteriaVMlist = teamCriterVMlist;
            teamCriteriaVMListComplete.teamFeedbackVMList = teamFeedbackVMList;


            ViewBag.TeamID = teamID;
            ViewBag.ContestID = contestID;
            return View(teamCriteriaVMListComplete);
        }


        public ActionResult CriteriaDetials(int teamID, int contestID, int criteriaID) {

            //get all scores for one criteria from all judges
            var getAllScoresForOneCriteria = context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.ContestId == contestID && cs.CriteriaId == criteriaID).ToList();
            List<CriteriaDetailVM> criteriaDetailVMList = new List<CriteriaDetailVM>();

            foreach (var item in getAllScoresForOneCriteria) {
                //get judge name for a contest
                string judgeName = context.AspNetUsers.Find(item.Judge_ID).UserName;


                //get score and comment from this judge 
                double? Score = null;
                string Comment = null;

                if (item.Submitted) {
                    Score = item.Score;
                    Comment = item.Comment;
                                    }
                criteriaDetailVMList.Add(new CriteriaDetailVM(item.Score, judgeName, Comment));
            }

            //get contest name
            ViewBag.ContestName = context.Contests.Find(contestID).Name;

            //get team name
            ViewBag.TeamName = context.Teams.Find(teamID).Name;
           
            //get criteria name
            ViewBag.CriteriaName = context.Criteria.Find(criteriaID).Name;

          
            //get the numbers of judges who haven't submitted
            int? JudgesNotSubmitted = criteriaDetailVMList.Where(x => x.Score == null).ToList().Count();

            if (JudgesNotSubmitted != 0)
            {
                ViewBag.JudgesNotSubmitted = JudgesNotSubmitted;

            }


            return View(criteriaDetailVMList);
        }
    }
}