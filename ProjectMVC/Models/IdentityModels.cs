using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System;
using System.Data.Entity.Infrastructure;

namespace ProjectMVC.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        [DefaultValue("unauthenticated") ]
        public string Status { get; set; }
        public byte[] Image { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime LastLogin { get; set; }
        public bool FirstLogin { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EmpAdminUser> Empadminuser { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<BookStatus> Bookstatus { get; set; }
        public DbSet<Notifications> Notifications { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public int SaveChanges(string _id, int level)
        {
            var modifiedEntities = ChangeTracker.Entries()
                .Where(p => p.State == EntityState.Modified).ToList();
            var now = DateTime.UtcNow;
            var addedEntities = ChangeTracker.Entries()
                .Where(p => p.State == EntityState.Added).ToList();
            if (modifiedEntities != null)
            {
                foreach (var change in modifiedEntities)
                {
                    string tableName = change.Entity.GetType().Name;
                    if (tableName.Contains("ApplicationUser"))
                    {
                        string Name = change.OriginalValues["FirstName"].ToString() + " " + change.OriginalValues["LastName"].ToString();
                        string body;
                        if (change.OriginalValues["IsDeleted"].ToString() != change.CurrentValues["IsDeleted"].ToString())
                        {
                            body = $"Removed {Name} from ";
                            ApplicationDbContext db = new ApplicationDbContext();
                            string id = change.CurrentValues["Id"].ToString();
                            var user = db.Users.SingleOrDefault(a => a.Id == id);
                            int role = int.Parse(user.Roles.SingleOrDefault(a => a.UserId == id).RoleId.ToString());
                            if (role == 2) { body += "Admin"; }
                            else if (role == 3) { body += "Employee"; }
                        }
                        else
                        {
                            body = "Changed ";
                            foreach (var prop in change.OriginalValues.PropertyNames)
                            {
                                if (change.OriginalValues[prop] != null && change.CurrentValues[prop] != null)
                                {
                                    var originalValue = change.OriginalValues[prop].ToString();
                                    var currentValue = change.CurrentValues[prop].ToString();
                                    if (originalValue != currentValue)
                                    {
                                        body += prop + ",";
                                    }
                                }

                            }
                            body += $"for {Name}";
                        }
                        Notifications not = new Notifications()
                        {
                            UserId = _id,
                            Body = body,
                            Date = now,
                            Level = level
                        };
                        Notifications.Add(not);
                    }
                    else if (tableName.Contains("Book"))
                    {
                        string Name = change.OriginalValues["Title"].ToString();
                        string body;
                        if (change.OriginalValues["IsDeleted"] != change.CurrentValues["IsDeleted"])
                        {
                            body = $"Removed '{Name}' from Books";
                        }
                        else
                        {
                            body = "Changed ";
                            foreach (var prop in change.OriginalValues.PropertyNames)
                            {
                                var originalValue = change.OriginalValues[prop].ToString();
                                var currentValue = change.CurrentValues[prop].ToString();
                                if (originalValue != currentValue)
                                {
                                    body += prop + ",";
                                }
                            }
                            body += $"for '{Name}' Book";
                        }
                        Notifications not = new Notifications()
                        {
                            UserId = _id,
                            Body = body,
                            Date = now,
                            Level = level
                        };
                        Notifications.Add(not);
                    }
                    else if (tableName.Contains("EmpAdminUser"))
                    {
                        string Name = change.OriginalValues["FirstName"].ToString() + " " + change.OriginalValues["LastName"].ToString();
                        string body;
                        body = "Changed ";
                        foreach (var prop in change.OriginalValues.PropertyNames)
                        {
                            if (change.OriginalValues[prop] != null && change.CurrentValues[prop] != null)
                            {
                                var originalValue = change.OriginalValues[prop].ToString();
                                var currentValue = change.CurrentValues[prop].ToString();
                                if (originalValue != currentValue)
                                {
                                    body += prop + ",";
                                }
                            }

                        }
                        body += $"for {Name}";
                        Notifications not = new Notifications()
                        {
                            UserId = _id,
                            Body = body,
                            Date = now,
                            Level = level
                        };
                        Notifications.Add(not);
                    }
                }
            }
            if (addedEntities != null)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                foreach (var change in addedEntities)
                {
                    string tableName = change.Entity.GetType().Name;
                    if (tableName == "EmpAdminUser")
                    {
                        string id = change.CurrentValues["ID"].ToString();
                        var user = db.Users.SingleOrDefault(a => a.Id == id);
                        string Name = user.FirstName + " " + user.LastName;
                        string body = $"Added {Name} to";
                        int role = int.Parse(user.Roles.SingleOrDefault(a => a.UserId == id).RoleId.ToString());
                        if (role == 2) { body += "Admin"; }
                        else if (role == 3) { body += "Employee"; }
                        Notifications not = new Notifications()
                        {
                            UserId = _id,
                            Body = body,
                            Date = now,
                            Level = level
                        };
                        Notifications.Add(not);
                    }
                    else if (tableName == "Book")
                    {
                        string Name = change.CurrentValues["Title"].ToString();
                        string body = $"Added '{Name}' to Books ";
                        Notifications not = new Notifications()
                        {
                            UserId = _id,
                            Body = body,
                            Date = now,
                            Level = level
                        };
                        Notifications.Add(not);
                    }
                }
            }
            return base.SaveChanges();
        }
        object GetPrimaryKeyValue(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
        }

        //public System.Data.Entity.DbSet<ProjectMVC.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<ProjectMVC.Models.ApplicationUser> ApplicationUsers { get; set; }

        ////public System.Data.Entity.DbSet<ProjectMVC.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}