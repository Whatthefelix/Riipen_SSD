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

            return View(ContestTeamVMList);

        }
        public ActionResult TeamScores(int teamID)
        {
            List<TeamCriteriaScoreVM> myTeamScore = new List<TeamCriteriaScoreVM>();
            var myScores = (from cs in context.CriteriaScores where cs.TeamId == teamID select cs).ToList();
            return View();
        }
    }
}