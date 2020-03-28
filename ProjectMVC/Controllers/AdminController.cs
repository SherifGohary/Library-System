using Microsoft.AspNet.Identity.Owin;
using ProjectMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net;
using Microsoft.AspNet.Identity;
using System.Drawing;

namespace ProjectMVC.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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


        // GET: Admin
        
        [Authorize(Roles = "BasicAdmin")]
        public ActionResult Index()
        {
           
            var adminid = db.Roles.Single(a => a.Id == "2").Users.ToList();
            List<EmpAdminUser> admins = new List<EmpAdminUser>();
            foreach (var item in adminid)
            {
                EmpAdminUser admin = db.Empadminuser.Include(a=>a.applicationUser).SingleOrDefault(a => a.ID == item.UserId && a.applicationUser.IsDeleted == false);
                if (admin != null)
                    admins.Add(admin);
            }

            return View(admins);
            
        }
        [Authorize(Roles ="BasicAdmin")]
        public ActionResult Reports()
        {


            BasicAdminReportsViewModel bar = new BasicAdminReportsViewModel();

            bar.AdminsNo = db.Roles.Single(a => a.Id == "2").Users.Count();
            bar.EmployeesNo = db.Roles.Single(a => a.Id == "3").Users.Count();
            bar.MembersNo = db.Roles.Single(a => a.Id == "4").Users.Count();
            bar.AvilableBooks = db.Books.Where(a => a.AvailableCopies > 0 && a.IsDeleted == false).Count();
            bar.BorrowedBooks = db.Activities.Where(a => (a.Status == 4)).Count();

            return View(bar);

        }

        //[Authorize(Roles = "BaseAdmin")]
        [Authorize(Roles = "BasicAdmin")]
        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "BaseAdmin")]

        public async Task<ActionResult> Create(EmpAdminUser admin,string password)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser adm = new ApplicationUser() { UserName = admin.applicationUser.Email, FirstName = admin.applicationUser.FirstName, LastName = admin.applicationUser.LastName, BirthDate = admin.applicationUser.BirthDate,LastLogin=DateTime.Now,IsDeleted=false, Email = admin.applicationUser.Email, EmailConfirmed=true, Status = "authenticated",FirstLogin=true };
                var result = await UserManager.CreateAsync(adm, password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(adm.Id, "Admin");


                    string id = db.Users.Where(a => a.Email == adm.Email).Select(a => a.Id).First().ToString();

                    EmpAdminUser emp = new EmpAdminUser() { ID = id, Salary = admin.Salary, HireDate = admin.HireDate };
                    db.Empadminuser.Add(emp);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }

            return View(admin);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

        }
    

        
        [Authorize(Roles = "BasicAdmin")]
        public JsonResult Delete(string AdminId)
        {
            var admin = db.Users.SingleOrDefault(a => a.Id == AdminId);
            admin.IsDeleted = true;
            admin.Email = $"{AdminId}@{AdminId}.com";
            admin.UserName = $"{AdminId}@{AdminId}.com";
            db.SaveChanges();
            return Json(new { Status = 1, Message = "Admin is deleted Successfully" }, JsonRequestBehavior.AllowGet);

        }
        // GET: Admins/Edit/2
        //[Authorize(Roles = "BaseAdmin")]
        [Authorize(Roles = "BasicAdmin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpAdminUser admin = db.Empadminuser.Include(a => a.applicationUser).SingleOrDefault(a => a.ID == id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "BaseAdmin")]

        public ActionResult Edit(string id, EmpAdminUser model)
        {
            if (ModelState.IsValid)
            {
                EmpAdminUser adminold = db.Empadminuser.Include(a => a.applicationUser).SingleOrDefault(a => a.ID == model.ID);
                adminold.HireDate = model.HireDate;
                adminold.Salary = model.Salary;
                adminold.applicationUser.FirstName = model.applicationUser.FirstName;
                adminold.applicationUser.LastName = model.applicationUser.LastName;
                var checkmail = db.Users.SingleOrDefault(a => a.Email == model.applicationUser.Email);
                if (adminold.applicationUser.Email == model.applicationUser.Email)
                {
                    db.SaveChanges();
                }
                else
                {
                    if (checkmail == null)
                    {
                        adminold.applicationUser.Email = model.applicationUser.Email;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("", "this email is already exists");
                        return View(model);
                    }
                }

                
                return RedirectToAction("Index");
            }

            return View(model);
        }
        //[Authorize(Roles = "Admin")]
        //[Authorize(Roles = "BaseAdmin")]

        [Authorize(Roles = "BasicAdmin,Admin")]
        public ActionResult MyProfile()
        {
            string _id = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(a => a.Id == _id);
            if (user.FirstLogin == true)
            {
                return RedirectToAction("ChangePassword", "account", new { id = _id });
            }
            else
            {

            EmpAdminUser admin = db.Empadminuser.Include(a => a.applicationUser).SingleOrDefault(a => a.ID == _id);
            return View(admin);
            }

        }
        // GET: Member/Edit/5
        [Authorize(Roles = "BasicAdmin,Admin")]
        public ActionResult EditProfile(string id)
        {
            EmpAdminUser admin = db.Empadminuser.Include(a => a.applicationUser).SingleOrDefault(a => a.ID == id);

            return View(admin);
        }
        
        public JsonResult GetNotifications()
        {
            var lists = db.Notifications.ToList();
            if (Session["id"] != null)
            {
                string id = User.Identity.GetUserId();
                var lastlogin = db.Users.SingleOrDefault(a => a.Id == id).LastLogin;
                if (User.IsInRole("BasicAdmin"))
                {
                    var list = db.Notifications.Include(a => a.appUser).Where(a => a.Date.CompareTo(lastlogin) > 0 && a.Level != 4).ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);

                }
                else if (User.IsInRole("Admin"))
                {
                    var list = db.Notifications.Include(a => a.appUser).Where(a => a.Date.CompareTo(lastlogin) > 0 && a.Level == 3).ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);

                }
                else if (User.IsInRole("Employee"))
                {
                    var list = db.Notifications.Include(a => a.appUser).Where(a => a.Date.CompareTo(lastlogin) > 0 && a.Level == 2).ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var list = db.Notifications.Include(a => a.appUser).Where(a => a.Date.CompareTo(lastlogin) > 0 && a.Level == 1).ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);

                }

            }
                return Json(lists, JsonRequestBehavior.AllowGet);
        }
        
        // POST: Member/Edit/5
        [HttpPost]
        public ActionResult EditProfile(EmpAdminUser model, HttpPostedFileBase img)
        {
            if(ModelState.IsValid)
            {
                
                string id = User.Identity.GetUserId();
                EmpAdminUser admin = db.Empadminuser.Include(a => a.applicationUser).SingleOrDefault(a => a.ID == id);
                var checkmail = db.Users.SingleOrDefault(a => a.Email == model.applicationUser.Email);                
                    admin.applicationUser.FirstName = model.applicationUser.FirstName;
                    admin.applicationUser.LastName = model.applicationUser.LastName;
                    admin.applicationUser.BirthDate = model.applicationUser.BirthDate;
                    admin.applicationUser.Address = model.applicationUser.Address;
                    admin.applicationUser.Email = model.applicationUser.Email;
                    admin.applicationUser.PhoneNumber = model.applicationUser.PhoneNumber;


                    if (img != null)
                    {
                        Image img1 = Image.FromStream(img.InputStream, true, true);
                        var img2 = Methods.imageToByteArray(img1);
                        admin.applicationUser.Image = img2;
                    }
                if (admin.applicationUser.Email == model.applicationUser.Email)
                {
                    db.SaveChanges();
                }
                else
                {
                    if (checkmail == null)
                    {
                        admin.applicationUser.Email = model.applicationUser.Email;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("", "this email is already exists");
                        return View(model);
                    }
                }
                
                // TODO: Add update logic here
                return RedirectToAction("MyProfile");
            }
            else
            {
                return View(model);
            }
        }
    }

}