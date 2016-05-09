using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Riipen_SSD
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



            routes.MapRoute(
                 "ContestDetails",
              "admin/{action}/{contestID}",
                 new {controller="admin", action = "Index", contestID = UrlParameter.Optional }

                );
            routes.MapRoute(
                "JudgeContestDetails",
                "judge/{action}/{contestId}",
                new { controller = "judge", action = "contest", contestId = "" }
                );

            routes.MapRoute(
           name: "Default",
           url: "{controller}/{action}/{id}",
           defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
       );
        }
    }
}
