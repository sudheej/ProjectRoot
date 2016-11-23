using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Root.Models
{
    [Table("Sentiment")]
    public class Sentiment
    {
        [Key]
        public long SentimentTagId { get; set; }
        public string Email { get; set; }
        public string SentimentText { get; set; }
        public string SentimentType { get; set; }
        public double SentimentScore { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}