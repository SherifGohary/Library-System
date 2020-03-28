using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectMVC.Models
{
    public class Category
    {
        public Category()
        {
            Books = new List<Book>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public List<Book> Books { get; set; }
    }
}