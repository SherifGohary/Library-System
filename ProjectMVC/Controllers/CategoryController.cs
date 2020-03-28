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

    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Category
        public ActionResult Index()
        {
            return View();
        }

        // GET: Category/Create
        [Authorize(Roles = "BasicAdmin,Admin")]

        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        public ActionResult Create(Category model)
        {
            try
            {
                Category cat = new Category() { Name = model.Name, IsDeleted = false };
                if(User.IsInRole("BasicAdmin"))
                    db.SaveChanges(Session["id"].ToString(), 3);
                else
                    db.SaveChanges(Session["id"].ToString(), 4);
                    // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Category/Edit/5
        [Authorize(Roles = "BasicAdmin,Admin")]

        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Category/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Category model)
        {
            try
            {
                Category cat = db.Categories.SingleOrDefault(a => a.ID == id);
                cat.Name = model.Name;
                // TODO: Add update logic here
                if (User.IsInRole("BasicAdmin"))
                    db.SaveChanges(Session["id"].ToString(), 3);
                else
                    db.SaveChanges(Session["id"].ToString(), 4);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Category/Delete/5
        [Authorize(Roles = "BasicAdmin,Admin")]

        public ActionResult Delete(int id)
        {
            Category cat = db.Categories.SingleOrDefault(a => a.ID == id);
            if (cat != null)
            {
                cat.IsDeleted = true;
                if (User.IsInRole("BasicAdmin"))
                    db.SaveChanges(Session["id"].ToString(), 3);
                else
                    db.SaveChanges(Session["id"].ToString(), 4);

            }
            return RedirectToAction("Index");
        }

    }
}
