using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Root.Models
{
    [Table("Clients")]
    public class Clients
    {
        [Key]
        public int ClientId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        public int ProjectId { get; set; }
        public int SentimentScore { get; set; }
    }
}