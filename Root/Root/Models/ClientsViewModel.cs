using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Root.Models
{
    public class ClientsViewModel
    {
       public List<Clients> allClients { get; set; }
       public List<Projects> allProjects { get; set; }
       public List<Sentiment> allSentiment { get; set; }
    }
}