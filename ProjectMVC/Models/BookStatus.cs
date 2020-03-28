using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectMVC.Models
{
    public class BookStatus
    {
        public BookStatus()
        {
            activities = new List<Activity>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Activity> activities { get; set; }
    }
}