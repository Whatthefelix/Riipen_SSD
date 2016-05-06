using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Riipen_SSD.DAL;
using Riipen_SSD.ViewModels.ParticipantViewModels;
using Riipen_SSD.ViewModels;
using PagedList;

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


        public ActionResult Index(String searchAContest, String sortContests, int? page)
        {
            string searchStringValue = "";
            string sortStringValue = "Latest contests";

            string UserID = User.Identity.GetUserId();
            List<ParticipantContestVM> participantVMList = new List<ParticipantContestVM>();
            var teamList = context.AspNetUsers.Find(UserID).Teams.ToList();

            foreach (var item in teamList)
            {
                var getContest = context.Contests.Where(c => c.Id == item.ContestId).FirstOrDefault();
                participantVMList.Add(new ParticipantContestVM(item.Id, getContest.Id, getContest.Name, getContest.StartTime, getContest.Location));

            }

            //get search result
            if (!String.IsNullOrEmpty(searchAContest))
            {
                participantVMList = participantVMList.Where(p => p.ContestName.ToUpper().Contains(searchAContest.ToUpper())).ToList();
                searchStringValue = searchAContest;
            }

            //sort result (default sort by start time)
            if (sortContests=="Name")
            {
                participantVMList = participantVMList.OrderBy(p => p.ContestName).ToList();
                sortStringValue = "Name";
            }
            else if(sortContests=="Location")
            {
                participantVMList = participantVMList.OrderBy(p => p.Location).ToList();
                sortStringValue = "Location";
            }
            else
            {
                participantVMList = participantVMList.OrderByDescending(p => p.StartTime).ToList();
            }


            ViewBag.SearchStringValue = searchStringValue;
            ViewBag.SortStringValue = sortStringValue;

            const int PAGE_SIZE = 10;
            int pageNumber = (page ?? 1);
            IEnumerable<ParticipantContestVM> newParticipantVMList = participantVMList;
            newParticipantVMList = newParticipantVMList.ToPagedList(pageNumber, PAGE_SIZE);

            return View(newParticipantVMList);
        }



        public ActionResult Contests(int contestID, String searchATeam, String sortTeams, int? page)
        {
            List<ContestTeamVM> ContestTeamVMList = new List<ContestTeamVM>();

            string searchStringValue = "";
            string sortStringValue = "Status";

            //get all criteria for one contset
            List<Criterion> getContestCriteria = _unitOfWork.Contests.Get(contestID).Criteria.ToList();

            //Get all teams in one contest
            var teams = context.Teams.Where(t => t.ContestId == contestID).ToList();
            
            string UserID = User.Identity.GetUserId();

            var yourteams = (from t in teams
                             from u in t.AspNetUsers
                             where u.Id == UserID && t.ContestId == contestID
                             select t).FirstOrDefault();

            int yourTeamID = yourteams.Id;

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

                //get the unsubmitted age
                foreach (var item in unsubmitedJudges)
                {
                    namesOfJudgeNotSubmitted.Add(context.AspNetUsers.Find(item.JudgeUserId).Email);
                }

                ContestTeamVMList.Add(new ContestTeamVM(team.Id, team.Name, CurrentScore, judgeNotSubmit,namesOfJudgeNotSubmitted));
                CurrentScore = null;
                judgeNotSubmit = null;
            }


            //search a team 
            if (!String.IsNullOrEmpty(searchATeam))
            {
                ContestTeamVMList = ContestTeamVMList.Where(c => c.TeamName.ToUpper().Contains(searchATeam.ToUpper())).ToList();
                searchStringValue = searchATeam;
            }

            //put your team at the top of team list and sort by status, then by sore
            if (sortTeams == "Name")
            {
                ContestTeamVMList = ContestTeamVMList.OrderByDescending(c => c.TeamID == yourTeamID).ThenBy(c=>c.TeamName).ToList();
                sortStringValue = "Name";
            }
            else
            {
                ContestTeamVMList = ContestTeamVMList.OrderByDescending(c => c.TeamID == yourTeamID).ThenBy(c=>c.JudgeNotSubmitted).ThenBy(c=>c.Score).ToList();

            }

            ViewBag.searchStringValue = searchStringValue;
            ViewBag.sortStringValue = sortStringValue;
            ViewBag.ContestID = contestID;
            ViewBag.ContestName = context.Contests.Find(contestID).Name;
            ViewBag.YourTeamID = yourTeamID;

            const int PAGE_SIZE = 10;
            int pageNumber = (page ?? 1);
            IEnumerable<ContestTeamVM> newContestTeamVMList = ContestTeamVMList;
            newContestTeamVMList = newContestTeamVMList.ToPagedList(pageNumber, PAGE_SIZE);

            return View(newContestTeamVMList);

        }



        public ActionResult TeamScores(int teamID, int contestID, int yourTeamID, double? totalScore)
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

                teamCriterVMlist.Add(new TeamCriteriaVM(item.Id, item.Name, item.Description, getAverageScoreForEachCriteria));                
            }

            //get the feedbacks for a team in contest
            List<TeamFeedbackVM> teamFeedbackVMList = new List<TeamFeedbackVM>();
            var getAllFeedbacskForATeamInAContest = context.Feedbacks.Where(f => f.ContestId == contestID && f.TeamId == teamID).ToList();

            //only display feedbacks when it is public and the judge has submitted the score
            foreach (var item in getAllFeedbacskForATeamInAContest)
            {
                //check if this judge has submitted his score
                if (context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.Judge_ID == item.JudgeUserId && cs.Submitted).ToList().Count() > 0) {
                    
                    //check if this judge write comment or not
                    if(!String.IsNullOrEmpty(item.PublicComment)||!String.IsNullOrEmpty(item.PrivateComment))
                    { 
                        //get the judge name for this feedback 
                        string JudgeName = context.AspNetUsers.Find(item.JudgeUserId).UserName;
                        
                        //check if this is your team, if so show you private feedback
                        if(teamID == yourTeamID)
                        {
                            teamFeedbackVMList.Add(new TeamFeedbackVM(JudgeName, item.PublicComment, item.PrivateComment));
                            ViewBag.YourTeam = true;
                        }
                        else
                        {
                            teamFeedbackVMList.Add(new TeamFeedbackVM(JudgeName, item.PublicComment, null));
                            ViewBag.YourTeam = false;
                        }
                    }
                   
                };
            }
          
            teamCriteriaVMListComplete.teamCriteriaVMlist = teamCriterVMlist;
            teamCriteriaVMListComplete.teamFeedbackVMList = teamFeedbackVMList;

            ViewBag.TeamName = _unitOfWork.Teams.Get(teamID).Name;
            ViewBag.ContestName = _unitOfWork.Contests.Get(contestID).Name;
            ViewBag.TeamID = teamID;
            ViewBag.ContestID = contestID;
            ViewBag.TotalScore = totalScore;
            return View(teamCriteriaVMListComplete);
        }


        public ActionResult CriteriaDetails(int teamID, int contestID, int criteriaID) {

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


        public ActionResult AllTeamMembersForATeam(int teamID)
        {          
            List<TeamMemberVM> TeamMemberVMList = context.Teams.Find(teamID).AspNetUsers.Select(x=> new TeamMemberVM {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email =x.Email
            }).ToList();

            ViewBag.TeamName = context.Teams.Find(teamID).Name;

            return View(TeamMemberVMList);
        }


    }
}