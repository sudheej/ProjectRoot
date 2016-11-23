using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Root.Models
{
    [Table("Tagger")]
    public class Tagger
    {
        [Key]
        public long TagId { get; set; }
        public string Email { get; set; }
        public string Tags { get; set; }
    }
}