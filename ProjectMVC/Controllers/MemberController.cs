using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Drawing;
using ProjectMVC.Models;
using System.Data.Entity;


namespace ProjectMVC.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        [Authorize (Roles ="BasicAdmin,Admin,Employee")]
        // GET: Member
        public ActionResult All(List<ApplicationUser> list)
        {
            string _id = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(a => a.Id == _id);
            if (user.FirstLogin == true)
            {
                return RedirectToAction("ChangePassword", "account", new { id = _id });
            }
            else
            {
                if (list == null)
                {
                    var memberid = db.Roles.Single(a => a.Id == "4").Users.ToList();
                    List<ApplicationUser> members = new List<ApplicationUser>();
                    foreach (var item in memberid)
                    {
                        ApplicationUser member = db.Users.SingleOrDefault(a => a.Id == item.UserId && a.IsDeleted == false);
                        if (member != null)
                            members.Add(member);
                    }

                    return View(members);
                }
                else
                {
                    return View(list);
                }
            }
            
        }

        public ActionResult All1(string Status)
        {
            var memberid = db.Roles.Single(a => a.Id == "4").Users.ToList();
            List<ApplicationUser> members = new List<ApplicationUser>();
            foreach (var item in memberid)
            {
                ApplicationUser member;
                if (Status == null)
                {
                     member = db.Users.SingleOrDefault(a => a.Id == item.UserId && a.IsDeleted == false);
                }else
                {
                     member = db.Users.SingleOrDefault(a => a.Id == item.UserId && a.IsDeleted == false && a.Status==Status);
                }
                if (member != null)
                    members.Add(member);
            }
            return RedirectToAction("All", new { list = members });
        }
        [HttpPost]
        public JsonResult Index1(string Prefix,string SearchBy)
        {
            var memberid = db.Roles.Single(a => a.Id == "4").Users.ToList();
            List<ApplicationUser> members = new List<ApplicationUser>();
            foreach (var item in memberid)
            {
                if (SearchBy == "Name")
                {
                    ApplicationUser member = db.Users.SingleOrDefault(a => a.Id == item.UserId && a.IsDeleted == false && a.FirstName.StartsWith(Prefix));
                    if (member != null)
                        members.Add(member);
                }
                else
                {
                    ApplicationUser member = db.Users.SingleOrDefault(a => a.Id == item.UserId && a.IsDeleted == false && a.Email.StartsWith(Prefix));
                    if (member != null)
                        members.Add(member);
                }

            }

            return Json(members, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult All(string Search, string SearchBy, string Status)
        {
            var memberid = db.Roles.Single(a => a.Id == "4").Users.ToList();
            List<ApplicationUser> members = new List<ApplicationUser>();
            if (Status.Length > 2)
            {
                foreach (var item in memberid)
                {
                    if (SearchBy == "Name")
                    {
                        ApplicationUser member = db.Users.SingleOrDefault(a => a.Id == item.UserId && a.IsDeleted == false && a.FirstName.Contains(Search) && a.Status == Status);
                        if (member != null)
                            members.Add(member);
                    }
                    else
                    {
                        ApplicationUser member = db.Users.SingleOrDefault(a => a.Id == item.UserId && a.IsDeleted == false && a.Email.Contains(Search) && a.Status == Status);
                        if (member != null)
                            members.Add(member);
                    }

                }
            }
            else
            {
                foreach (var item in memberid)
                {
                    if (SearchBy == "Name")
                    {
                        ApplicationUser member = db.Users.SingleOrDefault(a => a.Id == item.UserId && a.IsDeleted == false && a.FirstName.Contains(Search));
                        if (member != null)
                            members.Add(member);
                    }
                    else
                    {
                        ApplicationUser member = db.Users.SingleOrDefault(a => a.Id == item.UserId && a.IsDeleted == false && a.Email.Contains(Search));
                        if (member != null)
                            members.Add(member);
                    }

                }
            }

            return View(members);
        }
        [Authorize(Roles = "Member")]

        public ActionResult Profiles(string id)
        {
            if (id == null)
            {
                id = User.Identity.GetUserId();
                if (!User.IsInRole("Member"))
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                }
            }   
            var user = db.Users.SingleOrDefault(a => a.Id == id);
            return View(user);
        }
        [Authorize(Roles = "Member")]
        [HttpGet]
        public ActionResult ListBorrowedBooks(string id)//memberid
        {

            List<Activity> activites = db.Activities.Where(a => (a.MemberID == id) && (a.Status == 3)&& (a.Status == 1)).Include(b => b.book).ToList();

            return View(activites);
        }
        [HttpPost]
        public ActionResult ListBorrowedBooks(string month,string year,string status)//memberid
        {
            List<Activity> activites;
            int x =0;
            string id = User.Identity.GetUserId();
            if(int.TryParse(status,out x))
            {
                int s = int.Parse(status);
                if (int.TryParse(month, out x) && int.TryParse(year, out x))
                {
                    int m = int.Parse(month);
                    int y = int.Parse(year);
                    activites = db.Activities.Where(a => (a.MemberID == id) && (a.Status == s) && (a.StartDate.Month == m) && (a.StartDate.Year == y)).Include(b => b.book).ToList();

                }
                else if (int.TryParse(month, out x))
                {
                    int m = int.Parse(month);
                    activites = db.Activities.Where(a => (a.MemberID == id) && (a.Status == s) && (a.StartDate.Month == m)).Include(b => b.book).ToList();

                }
                else if (int.TryParse(year, out x))
                {
                    int y = int.Parse(year);
                    activites = db.Activities.Where(a => (a.MemberID == id) && (a.Status == s) && (a.StartDate.Year == y)).Include(b => b.book).ToList();
                }
                else
                {
                    activites = db.Activities.Where(a => (a.MemberID == id) && (a.Status == s)).Include(b => b.book).ToList();
                }

            }
            else
            {
                activites = db.Activities.Where(a => (a.MemberID == id) && (a.Status == 3)&& (a.Status == 1)).Include(b => b.book).ToList();
            }

            return View(activites);
        }
        public ActionResult RetrieveImage(string id)

        {
            
            byte[] cover = db.Users.Single(a => a.Id == id).Image;

            if (cover != null)

            {

                return File(cover, "image/jpg");

            }

            else

            {

                return null;

            }

        }

        // GET: Member/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Member/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Member/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [Authorize(Roles = "BasicAdmin")]
        public ActionResult Edit(string id)
        {
            var user = db.Users.SingleOrDefault(a => a.Id == id);

            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(string id, ApplicationUser model)
        {
            var user = db.Users.Single(a => a.Id == id);

            return View(user);
        }

        // GET: Member/Edit/5
        [Authorize(Roles = "Member")]
        public ActionResult EditProfile(string id)
        {
            var user = db.Users.SingleOrDefault(a => a.Id == id);
            ViewBag.status = user.Status;
            return View(user);
        }

        // POST: Member/Edit/5
        [HttpPost]
        public ActionResult EditProfile(string id, ApplicationUser model,HttpPostedFileBase img)
        {
            
                var user = db.Users.Single(a => a.Id == id);
            if (User.IsInRole("Member"))
            {
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
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                db.SaveChanges(id, 4);
            }
            else if (User.IsInRole("BasicAdmin"))
            {
                user.Status = model.Status;
                db.SaveChanges(id, 2);
            }
            else
            {
                user.Status = model.Status;
                db.SaveChanges(id, 1);
            }
                // TODO: Add update logic here
                

                return RedirectToAction($"Profiles/{id}");
            //}
            //catch
            //{
            //    return View();
            //}
        }
        [Authorize(Roles = "Member")]
        public ActionResult CurrentBorrow(string id)
        {
            var act = db.Activities.Include(a => a.book).Include(a => a.applicationUser).Where(a=>a.Status==4).ToList();
            var count= db.Activities.Where(a => a.Status == 3).GroupBy(a => a.BookID).Select(b => b.Count()).ToList();
            var list = act.Zip(count, (a, n) => new { Title = a.book.Title, ReturnDate = a.EndDate, Count = n.ToString() });
            List<BorrowingBooks> list1 = new List<BorrowingBooks>();
            foreach (var item in list)
            {
                list1.Add(new BorrowingBooks() { Title = item.Title, EndDate = item.ReturnDate, Count = int.Parse(item.Count) });
            }

            return View(list1);
        }

        // GET: Member/Delete/5
        [Authorize(Roles = "BasicAdmin,Employee")]
        public ActionResult Delete(string id)
        {
            var member = db.Users.SingleOrDefault(a => a.Id == id);
            member.IsDeleted = true;
            member.Email=$"{id}@{id}.com";
            member.UserName = $"{id}@{id}.com";
            db.SaveChanges();
            return RedirectToAction("All");
        }

        // POST: Member/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
