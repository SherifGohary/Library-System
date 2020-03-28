using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMVC.Models
{
    public class Notifications
    {
        public int ID { get; set; }
        [ForeignKey("appUser")]
        public string UserId { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public int Level { get; set; }
        public ApplicationUser appUser { get; set; }

    }
}