using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Root.Models;

namespace Root.Controllers
{
    public class ProjectsController : Controller
    {
        // GET: Projects
        public ActionResult Index()
        {
            ClientsContext clientContext = new ClientsContext();
            List<Projects> projectList = clientContext.Projects.ToList();
            return View(projectList);
        }
        public ActionResult getProject(int Id)
        {
            ClientsContext clientcontext = new ClientsContext();
            Projects projectinfo = clientcontext.Projects.Single(x => x.ProjectId == Id);
            double sentiScoreCurrentProject = 0.0;

            int count = 0;
            using (var db = new ClientsContext())
            {
                var query = (from senti in db.Sentiment
                             join clie in db.Clients on senti.Email equals clie.Email
                             join prj in db.Projects on clie.ProjectId equals prj.ProjectId
                             where prj.ProjectId == Id
                             select senti).ToList();

                foreach (var item in query)
                {
                    sentiScoreCurrentProject += item.SentimentScore;
                    count += 1;
                }

                ViewData["ProjectScore"] = (sentiScoreCurrentProject / count) * 100;

                var clientReport = (from clie in db.Clients
                                    where clie.ProjectId == Id
                                    select clie).ToList();
                string dummy = "";
                foreach (var sarath in clientReport)
                {
                    dummy = dummy + sarath.Name + "-" + sarath.SentimentScore + ",";
                }

                ViewData["ClientReport"] = dummy;

                return View(projectinfo);
            }
        }
            

    
    }
}