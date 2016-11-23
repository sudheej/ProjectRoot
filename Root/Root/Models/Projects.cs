using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Root.Models
{
    [Table("Projects")]
    public class Projects
    {
        [Key]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectManager { get; set; }
        public string Department { get; set; }
        public double ProjectSentiment { get; set; }
        public string CostCode { get; set; }
    }
}