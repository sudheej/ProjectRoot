using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Root.Models;
using System.Text;

namespace Root.Controllers
{
    public class ClientsController : Controller
    {
        // GET: Clients
        public ActionResult Index()
        {
            ClientsContext clientcontext = new ClientsContext();
            ClientsViewModel vm = new ClientsViewModel();
            vm.allClients = clientcontext.Clients.ToList();
            vm.allProjects = clientcontext.Projects.ToList();
          
            return View(vm);
        }

        
    
        public ActionResult getClient(int Id, string srch)
        {
            ViewData["ClientScore"] = 0.0;
            ViewData["ClientTags"] = "";
            ViewData["SearchString"] = srch;
            string tempBucketforSearch = "";
            using (var db = new ClientsContext())
            {
                var resultset = (from senti in db.Sentiment
                                 join clie in db.Clients on senti.Email equals clie.Email
                                 where clie.ClientId == Id && senti.SentimentText.Contains(srch)
                                 select senti).ToList();

                foreach (var itemresults in resultset)
                {
                    tempBucketforSearch = tempBucketforSearch + itemresults.SentimentText + "#" +itemresults.SentimentType+ "|";
                   
                }
                ViewData["searchResults"] = tempBucketforSearch;

            }
            double sentiScoreCurrentClient = 0.0;
            string tempBucketforTag = "";
            int count = 0;
            using (var db = new ClientsContext())
            {
                var query = (from senti in db.Sentiment
                             join clie in db.Clients on senti.Email equals clie.Email
                             where clie.ClientId == Id
                             select senti).ToList();

                foreach (var item in query)
                {
                    sentiScoreCurrentClient += item.SentimentScore;
                    count += 1;
                }

                ViewData["ClientScore"] = (sentiScoreCurrentClient / count) * 100;

                Clients c = (from clie in db.Clients
                             where clie.ClientId == Id
                             select clie).First();
                c.SentimentScore = Convert.ToInt16((sentiScoreCurrentClient / count) * 100);
                db.SaveChanges();

                var tagquery = (from tagger in db.Tagger
                                join clie in db.Clients on tagger.Email equals clie.Email
                                where clie.ClientId == Id
                                select tagger).ToList();


                foreach (var itemx in tagquery)
                {
                    tempBucketforTag = tempBucketforTag + itemx.Tags + ",";
                    
                }
                ViewData["ClientTags"] = tempBucketforTag;

                ClientsContext clientcontext = new ClientsContext();
                Clients Client = clientcontext.Clients.Single(x => x.ClientId == Id);
               
                    return View(Client);
                
            }


        }
        public ActionResult addClient()
        {
            ClientsContext clientcontext = new ClientsContext();
            List<Projects> projectlist = clientcontext.Projects.ToList();
            return View(projectlist);

        }
        [HttpPost]
        public ActionResult addClientResult(FormCollection form)
        {
            string N = form["txtName"].ToString();
            string E = form["txtEmail"].ToString();
            string P = form["txtProjectId"].ToString();
            ClientsContext cc = new ClientsContext();
            Clients client = new Clients()
            {
                Name = N,
                Email = E,
                ProjectId = Convert.ToInt16(P),
                SentimentScore = 10
            };
            try {
                cc.Clients.Add(client);
                cc.SaveChanges();
                TempData["notice"] = "Successfully registered " + N + " to client list";
            }
            catch
            {
                TempData["notice"] = "RED";
            }
            return RedirectToAction("Index");
           // return View(client);
        }
    }
}