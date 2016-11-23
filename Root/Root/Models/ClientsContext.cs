using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Root.Models
{
    public class ClientsContext : DbContext
    {
        public DbSet<Clients> Clients { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<Sentiment> Sentiment { get; set; }
        public DbSet<Tagger> Tagger { get; set; }
    }
}