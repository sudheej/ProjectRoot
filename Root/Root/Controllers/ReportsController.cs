using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Root.Models;
using System.Web.Helpers;

namespace Root.Controllers
{
   
    public class ReportsController : Controller
    {
        // GET: Reports
        [HttpGet]
        public ActionResult Index()
        {
            return View();

        }

        [HttpGet]
        public ActionResult Display()
        {
            ClientsContext clientcontext = new ClientsContext();
            List<Clients> allclients = clientcontext.Clients.ToList();
            List<string> xval = new List<string>();
            List<string> yval = new List<string>();

            foreach (Clients item in allclients)
            {
                xval.Add(item.Name);
                yval.Add(item.SentimentScore.ToString());
            }



            var dataForChart = allclients;

            var chart = new Chart(width: 700, height: 700)
            .AddSeries(chartType: "pie",
                            xValue: xval,
                            yValues: yval)
                            .GetBytes("png");

            return File(chart, "image/png");
        }

        public ActionResult DisplayProjectChart()
        {
            ClientsContext clientcontext = new ClientsContext();
            List<Projects> allclients = clientcontext.Projects.ToList();
            List<string> xval = new List<string>();
            List<string> yval = new List<string>();

            foreach (Projects item in allclients)
            {
                xval.Add(item.ProjectName);
                yval.Add(item.ProjectSentiment.ToString());
            }



            var dataForChart = allclients;

            var chart = new Chart(width: 500, height: 500)
            .AddSeries(chartType: "column",
                            xValue: xval,
                            yValues: yval)
                            .GetBytes("png");

            return File(chart, "image/png");
        }
    }
}