using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectMVC.Models;
using Microsoft.AspNet.Identity;
using System.Drawing;


namespace ProjectMVC.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Books
        
        public ActionResult Index()
        {
            string _id = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(a => a.Id == _id);
            if (user.FirstLogin == true)
            {
             return   RedirectToAction("ChangePassword","account", new { id = _id });

            }
            else
            {
                var books = db.Books.Where(a => a.AvailableCopies >= 1 && a.IsDeleted == false).Include(b => b.Category);
                return View(books.ToList());
            }
        }
        public ActionResult NewBooks()
        {
            string _id = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(a => a.Id == _id);
            if (user.FirstLogin == true)
            {
                return RedirectToAction("ChangePassword", "account", new { id = _id });

            }
            else
            {
                var books = db.Books.Include(a => a.Category).Where(a => a.AvailableCopies >= 1 && a.IsDeleted == false).OrderByDescending(a => a.ArriveDate).Take(10).ToList();
                return View(books);
            }
        }
        [Authorize (Roles ="Employee")]
        public ActionResult Borrow(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }
        [HttpPost]
        public ActionResult Borrow(Book book,string duration,string email)
        {

            var query = db.Users.SingleOrDefault(a => a.Email == email);
            
            if (query != null&&query.Status== "authenticated" && query.EmailConfirmed==true)
            {

                string id = db.Users.SingleOrDefault(a => a.Email == email).Id.ToString();
                if (id != null)
                {
                    var result = db.Activities.SingleOrDefault(a => a.MemberID == id && a.BookID == book.ID && a.Status == 4);
                    var result2 = db.Users.SingleOrDefault(a => a.Id == id && a.Status == "Block");
                    var result3 = db.Books.SingleOrDefault(a => a.AvailableCopies < 1 && a.ID == book.ID);

                    if (result == null && result2 == null && result3 == null)
                    {
                        Book oldbook = db.Books.SingleOrDefault(a => a.ID == book.ID);
                        oldbook.AvailableCopies -= 1;
                        oldbook.BorrowingTimes += 1;
                        Activity act = new Activity() { BookID = book.ID, MemberID = id, Duration = int.Parse(duration), StartDate = DateTime.Now, EndDate = DateTime.Now.Date.AddDays(double.Parse(duration)), Status = 4 };
                        db.Activities.Add(act);
                        db.SaveChanges();
                    }
                    else
                    {
                        if (result != null)
                        {
                            ModelState.AddModelError("", "This Member Is Already Borrowing This Book Now..!!");
                        }
                        else if (result2 != null)
                        {
                            ModelState.AddModelError("", "This Member Is Blocked Now..!!");
                        }
                        else if (result3 != null)
                        {
                            ModelState.AddModelError("", "This Book is not available for borrowing ..!!");
                        }

                        return View(book);
                    }

                }
            }
            else
            {
                ModelState.AddModelError("", "This email not found or still unverified");
                return View(book);
            }
            return RedirectToAction("Index");



        }
        [Authorize (Roles ="Employee")]
        public ActionResult Read(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }
        [HttpPost]
        public ActionResult Read(Book book, string duration, string email)
        {
            var query = db.Users.SingleOrDefault(a => a.Email == email);
            if (query != null)
            {
                string id = db.Users.SingleOrDefault(a => a.Email == email).Id.ToString();
                if (id != null)
                {
                    var result = db.Activities.SingleOrDefault(a => a.MemberID == id && a.BookID == book.ID && (a.Status == 2 || a.Status == 4));

                    if (result == null)
                    {
                        Book oldbook = db.Books.SingleOrDefault(a => a.ID == book.ID);
                        oldbook.AvailableCopies -= 1;
                        Activity act = new Activity() { BookID = book.ID, MemberID = id, Duration = int.Parse(duration), StartDate = DateTime.Now, EndDate = DateTime.Now.Date.AddDays(double.Parse(duration)), Status = 2 };
                        db.Activities.Add(act);
                        db.SaveChanges();
                    }
                    else
                    {
                        if (result != null)
                        {
                            ModelState.AddModelError("", "This Member Is Already Reading or Borrowing This Book Now..!!");
                        }

                        return View(book);
                    }
                }
            }
            else
            {

                ModelState.AddModelError("", "This email not found");
                return View(book);
            }
            return RedirectToAction("Index");
        }
        [Authorize (Roles ="Employee")]
        public ActionResult ReturnBook(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity act = db.Activities.SingleOrDefault(a => a.ID == id);
            if (act == null)
            {
                return HttpNotFound();
            }
            else
            {
                int _id = act.BookID;
                Book oldbook = db.Books.SingleOrDefault(a => a.ID == _id);
                oldbook.AvailableCopies += 1;
                if (act.Status == 2)
                {
                    act.Status = 1;
                }
                else
                {
                    act.Status = 3;
                }
                
                db.SaveChanges();
            }
            return RedirectToAction("History","Employee");

        }
        //[HttpPost]
        //public ActionResult ReturnBook(Book book, string email)
        //{

        //    string id = db.Users.Where(a => a.Email == email).Select(a => a.Id).First().ToString();
        //    if (id != null)
        //    {
        //        var result = db.Activities.SingleOrDefault(a => a.MemberID == id && a.BookID == book.ID && a.Status == 4);
                
        //        if (result != null )
        //        {
        //            Book oldbook = db.Books.SingleOrDefault(a => a.ID == book.ID);
        //            oldbook.AvailableCopies += 1;

        //            result.Status = 3;
        //            db.SaveChanges();
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "that Member didnt borrow this book");

        //            return View(book);
        //        }
        //    }
        //    return RedirectToAction("Index");


        //}
        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        [Authorize (Roles ="Admin")]
        public ActionResult Create()
        {
            string _id = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(a => a.Id == _id);
            if (user.FirstLogin == true)
            {
                return RedirectToAction("ChangePassword", "account", new { id = _id });
            }
            else
            {

                ViewBag.CID = new SelectList(db.Categories, "ID", "Name");
                return View();
            }
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book model,HttpPostedFileBase img)
        {
            if (ModelState.IsValid)
            {
                if (img != null)
                {
                    Image img1 = Image.FromStream(img.InputStream, true, true);
                    var img2 = Methods.imageToByteArray(img1);
                    var book = new Book() { Title = model.Title, Author = model.Author, Publisher = model.Publisher, PublishDate = model.PublishDate, Quantity = model.Quantity, Pages = model.Pages, AvailableCopies = model.Quantity, ShelfNumber = model.ShelfNumber, ArriveDate = model.ArriveDate, Image = img2, CID = model.CID, IsDeleted = false,BorrowingTimes=0 };
                    book.Image = img2;
                    db.Books.Add(book);
                }
                db.SaveChanges(User.Identity.GetUserId(),4);
                return RedirectToAction("Index");
            }

            ViewBag.CID = new SelectList(db.Categories, "ID", "Name", model.CID);
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(string SearchBy, string Search)
        {
            List<Book> books;
            if (SearchBy == "Title")
            {
                books = db.Books.Where(a => a.Title.Contains(Search) &&a.IsDeleted==false).Include(a=>a.Category).ToList();
            }
            else if (SearchBy == "Author")
            {
                books = db.Books.Where(a => a.Author.Contains(Search) && a.IsDeleted == false).Include(a => a.Category).ToList();
            }
            else if (SearchBy == "Publisher")
            {
                books = db.Books.Where(a => a.Publisher.Contains(Search) && a.IsDeleted == false).Include(a => a.Category).ToList();
            }
            else
            {
                books = db.Books.Where(a => a.Category.Name.Contains(Search) && a.IsDeleted == false).Include(a => a.Category).ToList();
            }
            return View(books);
        }


        [HttpPost]
        public JsonResult Index1(string Prefix,string searchby)
        {
            List<Book> books;
            if (searchby == "Title")
            {
                books = db.Books.Where(a => a.Title.StartsWith(Prefix)).ToList();
            }else if (searchby=="Author")
            {
                books = db.Books.Where(a => a.Author.StartsWith(Prefix)).ToList();
            }else if (searchby=="Publisher")
            {
                books = db.Books.Where(a => a.Publisher.StartsWith(Prefix)).ToList();
            }
            else 
            {
                var cat = db.Categories.Where(a => a.Name.StartsWith(Prefix)).ToList();
                return Json(cat, JsonRequestBehavior.AllowGet);

            }
            return Json(books, JsonRequestBehavior.AllowGet);
        }

        // GET: Books/Edit/5
        [Authorize (Roles ="BasicAdmin,Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.CID = new SelectList(db.Categories, "ID", "Name", book.CID);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Book model,HttpPostedFileBase img)
        {
            if (ModelState.IsValid)
            {
                if (img != null)
                {
                    Image img1 = Image.FromStream(img.InputStream, true, true);
                    var img2 = Methods.imageToByteArray(img1);
                    var book = db.Books.SingleOrDefault(a => a.ID == model.ID);
                    book.Title = model.Title;
                    book.Author = model.Author;
                    book.Publisher = model.Publisher;
                    book.PublishDate = model.PublishDate;
                    book.ArriveDate = model.ArriveDate;
                    var plus =model.Quantity- book.Quantity;
                    book.Quantity = model.Quantity;
                    book.AvailableCopies += plus;
                    book.ShelfNumber = model.ShelfNumber;
                    book.Image = img2;
                    book.CID = model.CID;
                    if (User.IsInRole("BasicAdmin"))
                    {
                        db.SaveChanges(User.Identity.GetUserId(),3);
                    }
                    else
                    {
                        db.SaveChanges(User.Identity.GetUserId(), 4);
                    }
                }

                return RedirectToAction("Index");
            }
            ViewBag.CID = new SelectList(db.Categories, "ID", "Name", model.CID);
            return View(model);
        }

        // GET: Books/Delete/5
        //[Authorize (Roles ="BasicAdmin,Admin")]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Book book = db.Books.Find(id);
        //    if (book == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(book);
        //}

        // POST: Books/Delete/5
        [Authorize(Roles = "BasicAdmin,Admin")]
        [ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.SingleOrDefault(a => a.ID == id);
            if (book != null)
            {
                book.IsDeleted = true;
                if (User.IsInRole("BasicAdmin"))
                {
                    db.SaveChanges(User.Identity.GetUserId(),3);
                }
                else
                {
                    db.SaveChanges(User.Identity.GetUserId(), 4);
                }


            }
            return RedirectToAction("Index");
        }
        public ActionResult RetrieveImage(int id)

        {

            byte[] cover = db.Books.Single(a => a.ID == id).Image;

            if (cover != null)

            {

                return File(cover, "image/jpg");

            }

            else

            {

                return null;

            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
