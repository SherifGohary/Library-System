using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMVC.Models
{
    public class Activity
    {
        public int ID { get; set; }
        [Required]
        [ForeignKey("applicationUser")]
        public string MemberID { get; set; }
        [Required]
        [ForeignKey("book")]
        public int BookID { get; set; }
        [Required]
        [ForeignKey("bookStatus")]
        public int Status { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [DefaultValue(7)]
        public int Duration { get; set; }
        public DateTime EndDate { get; set; }

        public ApplicationUser applicationUser { get; set; }
        public Book book { get; set; }
        public BookStatus bookStatus { get; set; }
    }
}