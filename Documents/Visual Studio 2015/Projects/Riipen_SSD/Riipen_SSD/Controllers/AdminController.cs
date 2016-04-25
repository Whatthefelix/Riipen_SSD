using Riipen_SSD.DAL;
using Riipen_SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Riipen_SSD.Controllers
{
    public class AdminController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public AdminController()
        {
            _unitOfWork = new UnitOfWork(new Riipen_SSDEntities());
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CreateContest()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateContest(ContestVM contest)
        {
            var contestName = contest.ContestName;
            var date = contest.Date;
            var location = contest.Location;
            var criteria = contest.Criteria;

            ICollection<Criterion> criteriaList = criteria.Select(x => new Criterion() { Description = x.Description, Name = x.Criteria }).ToList();

            _unitOfWork.Contests.Add(new Contest()
            {
                Criteria = criteriaList,
                StartTime = date,
                Location = location,
                Name = contestName,
            });

            _unitOfWork.Complete();

            return View();
        }
    }
}