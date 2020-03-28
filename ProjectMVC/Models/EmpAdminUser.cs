using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMVC.Models
{
    public class EmpAdminUser
    {
        [Key]
        [ForeignKey("applicationUser")]
        public string ID { get; set; }
        [Required]
        [Range(minimum: 1, maximum: 30000, ErrorMessage = "Should be more than 0")]

        public int Salary { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public string HireDate { get; set; }

        public ApplicationUser applicationUser { get; set; }
    }
}