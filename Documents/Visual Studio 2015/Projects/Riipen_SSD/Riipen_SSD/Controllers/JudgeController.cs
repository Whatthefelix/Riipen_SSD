using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Riipen_SSD.DAL;
using Microsoft.AspNet.Identity;
using Riipen_SSD.Models.ViewModels;

namespace Riipen_SSD.Controllers
{
    public class JudgeController : Controller
    {
        Riipen_SSDEntities context = new Riipen_SSDEntities();

        private IUnitOfWork _unitOfWork;

        public JudgeController()
        {

            _unitOfWork = new UnitOfWork(new Riipen_SSDEntities());

        }

        // GET: Judge
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
            ViewBag.ContestName = _unitOfWork.Contests.Get(contestID).Name;
            return View(teams);

        }


        public ActionResult team(int teamID)
        {

            int contestID = _unitOfWork.Teams.Get(teamID).ContestId;
            List<Criterion> getContestCriteria = _unitOfWork.Contests.Get(contestID).Criteria.ToList();

            List<CriteriaScore> getCriteriaScores = _unitOfWork.CriteriaScores.Find(cs => cs.TeamId == teamID & cs.ContestId == contestID).ToList();
            List<CriteriaScoreVM> CriteriaScoreVMList = new List<CriteriaScoreVM>();

            if (getCriteriaScores.Count() != 0)
            {
                var CriteriaScores = from g in getContestCriteria
                                     from cs in getCriteriaScores
                                     where g.ContestId == cs.ContestId && cs.TeamId == teamID && g.Id == cs.ContestId
                                     select new
                                     {
                                         CriteriaId = g.ContestId,
                                         Name = g.Name,
                                         Desciption = g.Description,
                                         Score = cs.Score,
                                         Comment = cs.Comment
                                     };

            }
            else {
                var CriteriaScores = from g in getContestCriteria
                                     select new
                                     {
                                         CriteriaId = g.ContestId,
                                         Name = g.Name,
                                         Desciption = g.Description,
                                         Score = 0,
                                         Comment = ""
                                     };




                foreach (var CriteriaScore in CriteriaScores)
                {
                    CriteriaScoreVMList.Add(new CriteriaScoreVM(CriteriaScore.CriteriaId, CriteriaScore.Name, CriteriaScore.Desciption, CriteriaScore.Score, CriteriaScore.Comment));
                }


            }

            ViewBag.TeamID = teamID;
            return View(CriteriaScoreVMList);

        }


        public ActionResult EditCriteriaScore(int teamID, int criteriaID)
        {
            CriteriaScoreVM criteriaScoreItem = new CriteriaScoreVM();

            CriteriaScore criteriaScore = context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.CriteriaId == criteriaID).FirstOrDefault();
            Criterion criterion = _unitOfWork.Criteria.Get(criteriaID);


            if (criteriaScore != null)
            {
                criteriaScoreItem.Criteria = criterion.Name;
                criteriaScoreItem.Score = criteriaScore.Score;
                criteriaScoreItem.Description = criterion.Description;
                criteriaScoreItem.Comment = criteriaScore.Comment;
            }
            else {
                criteriaScoreItem.Criteria = criterion.Name;
                criteriaScoreItem.Score = 0;
                criteriaScoreItem.Description = criterion.Description;
                criteriaScoreItem.Comment = "";
            }




            return View(criteriaScoreItem);


        }
    }
}