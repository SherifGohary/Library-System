using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Drawing;
using ProjectMVC.Models;
using System.Collections.Generic;
using System.Net;
using System.Data.Entity;

namespace ProjectMVC.Controllers
{
   [Authorize]
    public class EmployeeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public EmployeeController()
        {

        }
        public EmployeeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        // GET: Employee
        //[Authorize (Roles = "BasicAdmin")]
        [Authorize(Roles = "BasicAdmin,Admin")]

        public ActionResult Index()
        {
            string _id = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(a => a.Id == _id);
            if (user.FirstLogin == true)
            {
                return RedirectToAction("ChangePassword", "account", new { id = _id });
            }
            else
            {
                var EmployeeId = db.Roles.SingleOrDefault(a => a.Id == "3").Users.ToList();
                List<EmpAdminUser> Employees = new List<EmpAdminUser>();
                foreach (var item in EmployeeId)
                {
                    EmpAdminUser employee = db.Empadminuser.Include(a => a.applicationUser).SingleOrDefault(b => b.ID == item.UserId && b.applicationUser.IsDeleted == false);
                    if (employee != null)
                    {
                        Employees.Add(employee);
                    }

                }
                return View(Employees);
            }
        }
        [HttpPost]
        public JsonResult Index1(string Prefix)
        {
            var EmployeeId = db.Roles.SingleOrDefault(a => a.Id == "3").Users.ToList();
            List<EmpAdminUser> Employees = new List<EmpAdminUser>();
            foreach (var item in EmployeeId)
            {
                EmpAdminUser employee = db.Empadminuser.Include(a => a.applicationUser).SingleOrDefault(b => b.ID == item.UserId && b.applicationUser.IsDeleted==false && (b.applicationUser.FirstName + " " + b.applicationUser.LastName).StartsWith(Prefix));
                if (employee!=null)
                {
                    Employees.Add(employee);
                }

            }
            return Json(Employees, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Index(string Search)
        {
            var EmployeeId = db.Roles.SingleOrDefault(a => a.Id == "3").Users.ToList();
            List<EmpAdminUser> Employees = new List<EmpAdminUser>();
            foreach (var item in EmployeeId)
            {
                EmpAdminUser employee = db.Empadminuser.Include(a => a.applicationUser).SingleOrDefault(b => b.ID == item.UserId && b.applicationUser.IsDeleted == false && (b.applicationUser.FirstName + " " + b.applicationUser.LastName).Contains(Search));
                if (employee != null)
                {
                    Employees.Add(employee);
                }

            }
            return View(Employees);
        }

        // GET: Employee/Details/5
        [Authorize (Roles ="BasicAdmin,Admin,Employee")]
        public ActionResult Details(string id)
        {
            EmpAdminUser emp;
            if (id == null)
            {
                string nid = User.Identity.GetUserId();
                 emp = db.Empadminuser.Include(a => a.applicationUser).SingleOrDefault(a => a.ID == nid);
            }
            else
            {
                 emp = db.Empadminuser.Include(a => a.applicationUser).SingleOrDefault(a => a.ID == id);
            }
            if (emp == null)
            {
                return HttpNotFound();
            }
            return View(emp);
        }

        // GET: Employee/Create
        [Authorize (Roles ="BasicAdmin,Admin")]
        public ActionResult Create()
        {
            string _id = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(a => a.Id == _id);
            if (user.FirstLogin == true)
            {
                return RedirectToAction("ChangePassword", "account", new { id = _id });
            }
            return View(); 
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EmpAdminView model, HttpPostedFileBase img)
        {
            if (ModelState.IsValid&&img!=null)
            {
                Image img1 = Image.FromStream(img.InputStream, true, true);
                var img2 = Methods.imageToByteArray(img1);
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, BirthDate = model.BirthDate, Address = model.Address, PhoneNumber = model.PhoneNumber, Image = img2, LastLogin = DateTime.Now, IsDeleted = false, Status = "authenticated",FirstLogin=true,EmailConfirmed=true };
                var emp = new EmpAdminUser { Salary = model.Salary,HireDate=model.HireDate };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "Employee");
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    ApplicationDbContext db = new ApplicationDbContext();
                    string id = db.Users.Where(a => a.Email == user.Email).Select(a => a.Id).First().ToString();
                    emp.ID = id;
                    db.Empadminuser.Add(emp);
                    if (User.IsInRole("BasicAdmin"))
                    {
                        db.SaveChanges(User.Identity.GetUserId(),3);
                    }
                    else
                    {
                        db.SaveChanges(User.Identity.GetUserId(), 4);
                    }
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");                    
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        // GET: Employee/Edit/5
        [Authorize (Roles ="Employee")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                string nid = User.Identity.GetUserId();
                var user = db.Users.Single(a => a.Id == nid);
                return View(user);
            }
            else
            {
                var user = db.Users.Single(a => a.Id == id);
                return View(user);
            }

        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, ApplicationUser model, HttpPostedFileBase img)
        {
            try
            {
                var user = db.Users.SingleOrDefault(a => a.Id == id);

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.BirthDate = model.BirthDate;
                    user.Address = model.Address;
                    if (img != null)
                    {
                        Image img1 = Image.FromStream(img.InputStream, true, true);
                        var img2 = Methods.imageToByteArray(img1);
                        user.Image = img2;
                    }
                    var checkmail = db.Users.SingleOrDefault(a => a.Email == model.Email);
                    user.PhoneNumber = model.PhoneNumber;
                if (checkmail == null)
                {
                    user.Email = model.Email;
                }
                else
                {
                    if (user.Email == model.Email)
                    {
                        user.Email = model.Email;
                    }
                    else
                    {
                        ModelState.AddModelError("", "this email is already exists");
                        return View(model);
                    }
                   
                }
                // TODO: Add update logic here
                db.SaveChanges(User.Identity.GetUserId(),4);

                return RedirectToAction("Details");
            }
            catch
            {
                ModelState.AddModelError("", "invalid Data");

                return View(model);
            }
        }
        [Authorize (Roles ="BasicAdmin,Admin")]
        //Get
        public ActionResult AdminEdit(string id)
        {
            var user = db.Empadminuser.Include(b=>b.applicationUser).SingleOrDefault(a => a.ID == id);

            return View(user);
        }

        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminEdit(string id, EmpAdminUser model, HttpPostedFileBase img)
        {
            try
            {
                var user = db.Empadminuser.Include(b=>b.applicationUser).SingleOrDefault(a => a.ID == id);

                user.applicationUser.FirstName = model.applicationUser.FirstName;
                user.applicationUser.LastName = model.applicationUser.LastName;
                user.applicationUser.BirthDate = model.applicationUser.BirthDate;
                user.applicationUser.Address = model.applicationUser.Address;
                if (img != null)
                {
                    Image img1 = Image.FromStream(img.InputStream, true, true);
                    var img2 = Methods.imageToByteArray(img1);
                    user.applicationUser.Image = img2;
                }
                user.applicationUser.PhoneNumber = model.applicationUser.PhoneNumber;
                user.Salary = model.Salary;
                user.HireDate = model.HireDate;
                var checkmail = db.Users.SingleOrDefault(a => a.Email == model.applicationUser.Email);
                if (checkmail == null)
                {
                    user.applicationUser.Email = model.applicationUser.Email;
                }
                else
                {
                    if (user.applicationUser.Email == model.applicationUser.Email)
                    {
                        user.applicationUser.Email = model.applicationUser.Email;
                    }
                    else
                    {
                        ModelState.AddModelError("", "this email is already exists");
                        return View(model);
                    }
                }
                if (User.IsInRole("BasicAdmin"))
                {
                    db.SaveChanges(User.Identity.GetUserId(),3);
                }
                else
                {
                    db.SaveChanges(User.Identity.GetUserId(), 4);
                }
                // TODO: Add update logic here

                return RedirectToAction("index");
            }
            catch
            {
                ModelState.AddModelError("", "invalid Data");

                return View(model);
            }
        }
        [Authorize (Roles ="BasicAdmin,Admin,Employee")]
        public ActionResult History()
        {
            string _id = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(a => a.Id == _id);
            if (user.FirstLogin == true)
            {
                RedirectToAction("ChangePassword", new { id = _id });
            }
            var act = db.Activities.Include(a=>a.book).Include(a=>a.applicationUser).ToList();
            var status = db.Bookstatus;
           
            MultiSelectList items = new MultiSelectList(status, "ID", "Name");
            ViewBag.list = items;
            return View(act);
        }
        [HttpPost]
        public ActionResult History(FormCollection form)
        {
            ViewBag.YouSelected = form["list"];

            string selectedValues = form["list"];
            string[] arr = new string[0];
            if (selectedValues != null)
            {
                arr = selectedValues.Split(',');
            }
            ViewBag.list = GetStatus();
            var act = db.Activities.Include(a => a.book).Include(a => a.applicationUser).ToList();
            if (arr.Length == 1)
                {

                    int s1 = int.Parse(arr[0]);
                    act = db.Activities.Include(a => a.book).Include(a => a.applicationUser).Where(a => a.Status == s1).ToList();
                }
                else if (arr.Length == 2)
                {
                    int s1 = int.Parse(arr[0]);
                    int s2 = int.Parse(arr[1]);                
                    act = db.Activities.Include(a => a.book).Include(a => a.applicationUser).Where(a => a.Status == s1 || a.Status == s2).ToList();
                }
                else if (arr.Length == 3)
                {
                    int s1 = int.Parse(arr[0]);
                    int s2 = int.Parse(arr[1]);
                    int s3 = int.Parse(arr[2]);
                    act = db.Activities.Include(a => a.book).Include(a => a.applicationUser).Where(a => a.Status == s1 || a.Status == s2 || a.Status == s3).ToList();
                }
                else
                {
                    act = db.Activities.Include(a => a.book).Include(a => a.applicationUser).ToList();
                }



            return View(act);
        }
        private MultiSelectList GetStatus()
        {

            var status = db.Bookstatus;
            //List<SelectListItem> items = new List<SelectListItem>();
            //items.Add(new SelectListItem { Text = "All", Value = "All", Selected = true });
            //foreach (var item in status)
            //{
            //    items.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString() });
            //}
            

            return new MultiSelectList(status, "ID", "Name");

        }
        // GET: Employee/Delete/5
        [Authorize (Roles ="BasicAdmin,Admin")]
        public ActionResult Delete(string id)
        {
            var emp = db.Users.SingleOrDefault(a => a.Id == id);
            if (emp != null)
            {
                emp.IsDeleted = true;
                emp.Email = $"{id}@{id}.com";
                emp.UserName = $"{id}@{id}.com";
                if (User.IsInRole("BasicAdmin"))
                {
                    db.SaveChanges(User.Identity.GetUserId(),3);
                }
                else
                {
                    db.SaveChanges(User.Identity.GetUserId(), 4);
                }

            }
            
            return RedirectToAction("index");
        }

        //// POST: Employee/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    ApplicationUser applicationUser = db.Users.Find(id);
        //    db.Users.Remove(applicationUser);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
