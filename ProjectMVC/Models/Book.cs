using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;



namespace ProjectMVC.Models
{
    public class Book
    {
        
        public int ID { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "The FisrtName must be at least 3 characters long not more than 20.", MinimumLength = 3)]
        public string Title { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "The FisrtName must be at least 3 characters long not more than 20.", MinimumLength = 3)]
        public string Author { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "The FisrtName must be at least 3 characters long not more than 20.", MinimumLength = 3)]
        public string Publisher { get; set; }
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }
        [Required]
        [Range(minimum:1,maximum: 200,ErrorMessage = "Should be more than 0")]
        public int Quantity { get; set; }
        [Range(minimum:1,maximum:3000,ErrorMessage ="Should be more than 0")]
        public int Pages { get; set; }
        public int AvailableCopies { get; set; }
        [Required]
        public int ShelfNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime ArriveDate { get; set; }
        public byte[] Image { get; set; }
        [Required]
        [ForeignKey("Category")]
        public int CID { get; set; }
        public bool IsDeleted { get; set; }
        public int BorrowingTimes { get; set; }
        public Category Category { get; set; }
    }
}