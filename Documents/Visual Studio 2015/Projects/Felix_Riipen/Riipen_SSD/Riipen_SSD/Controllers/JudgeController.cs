﻿using System;
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
        SSD_RiipenEntities context = new SSD_RiipenEntities();

        private IUnitOfWork _unitOfWork;

      
        public JudgeController()
        {
          

            _unitOfWork = new UnitOfWork(new SSD_RiipenEntities());

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
            string UserID = User.Identity.GetUserId();
            int contestID = _unitOfWork.Teams.Get(teamID).ContestId;
            string TeamName = _unitOfWork.Teams.Get(teamID).Name;
            List<Criterion> getContestCriteria = _unitOfWork.Contests.Get(contestID).Criteria.ToList();
            List<CriteriaScore> getCriteriaScores = _unitOfWork.CriteriaScores.Find(cs => cs.TeamId == teamID & cs.ContestId == contestID && cs.Judge_ID == UserID).ToList();
            List<CriteriaScoreVM> CriteriaScoreVMList = new List<CriteriaScoreVM>();

            if (getCriteriaScores.Count() != 0)
            {
                foreach (var item in getContestCriteria) {
                    CriteriaScore criteriaScore = context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.CriteriaId == item.Id && cs.Judge_ID == UserID).FirstOrDefault();

                    double? AverageScore = null;
                    if (criteriaScore.Score!= null) { 
                        List<CriteriaScore> getAllScoresForOneCriteria = context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.CriteriaId == item.Id).ToList();
                       AverageScore = getAllScoresForOneCriteria.Average(s => s.Score);
                    }

                    CriteriaScoreVMList.Add(new CriteriaScoreVM(item.Id, item.Name, item.Description, AverageScore, criteriaScore.Comment));
                }
            }
            else {
                foreach (var item in getContestCriteria)
                {
                    CriteriaScore newCriteriaScore = new CriteriaScore();
                    newCriteriaScore.TeamId = teamID;
                    newCriteriaScore.CriteriaId = item.Id;
                    newCriteriaScore.ContestId = contestID;
                    newCriteriaScore.Judge_ID = User.Identity.GetUserId();
                    context.CriteriaScores.Add(newCriteriaScore);
                    CriteriaScoreVMList.Add(new CriteriaScoreVM(item.Id, item.Name, item.Description, null, null));
                    context.SaveChanges();
                }
            }

            ViewBag.TeamID = teamID;
            ViewBag.TeamName = TeamName;
            return View(CriteriaScoreVMList);

        }


        public ActionResult EditCriteriaScore(int teamID, int criteriaID)
        {
            string UserID = User.Identity.GetUserId();
            CriteriaScore criteriaScore = context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.CriteriaId == criteriaID && cs.Judge_ID == UserID).FirstOrDefault();
            Criterion criterion = _unitOfWork.Criteria.Get(criteriaID);
            CriteriaScoreVM criteriaScoreItem = new CriteriaScoreVM(criterion.Id, criterion.Name, criterion.Description, criteriaScore.Score, criteriaScore.Comment);
            ViewBag.TeamID = teamID;
            return View(criteriaScoreItem);

        }

        [HttpPost]
        public ActionResult EditCriteriaScore(int teamID, int criteriaID,string Comment, int Score) {
            string UserID = User.Identity.GetUserId();
            CriteriaScore criteriaScore = context.CriteriaScores.Where(cs => cs.TeamId == teamID && cs.CriteriaId == criteriaID && cs.Judge_ID == UserID).FirstOrDefault();
            criteriaScore.Score = Score;
            criteriaScore.Comment = Comment;
            context.SaveChanges();
            return RedirectToAction("Team", new { teamID= teamID });
        }

        

    }
}