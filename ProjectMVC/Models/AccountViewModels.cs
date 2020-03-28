using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace ProjectMVC.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    public class BasicAdminReportsViewModel
    {

        public int AdminsNo { get; set; }
        public int EmployeesNo { get; set; }
        public int MembersNo { get; set; }
        public int BooksNo { get; set; }
        public int AvilableBooks { get; set; }
        public int BorrowedBooks { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class EmpAdminView:RegisterViewModel
    {
        //this class you can add it in accountviewModels  
        [Required]
        [Range(minimum: 1, maximum: 30000, ErrorMessage = "Should be more than 0")]
        public int Salary { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public string HireDate { get; set; }
    }
    public class BorrowingBooks
    {
        public string Title { get; set; }
        public DateTime EndDate { get; set; }
        public int Count { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "The FisrtName must be at least 3 characters long not more than 20.", MinimumLength = 3)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The FisrtName must be at least 3 characters long not more than 20.", MinimumLength = 3)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [DataType(DataType.Text)]
        public string Address { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
