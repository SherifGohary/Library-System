namespace ProjectMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class start : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MemberID = c.String(nullable: false, maxLength: 128),
                        BookID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        Duration = c.Int(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.MemberID, cascadeDelete: true)
                .ForeignKey("dbo.Books", t => t.BookID, cascadeDelete: true)
                .ForeignKey("dbo.BookStatus", t => t.Status, cascadeDelete: true)
                .Index(t => t.MemberID)
                .Index(t => t.BookID)
                .Index(t => t.Status);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        BirthDate = c.DateTime(nullable: false),
                        Address = c.String(),
                        Status = c.String(),
                        Image = c.Binary(),
                        IsDeleted = c.Boolean(nullable: false),
                        FirstLogin = c.Boolean(nullable: false),
                        LastLogin = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 40),
                        Author = c.String(nullable: false, maxLength: 40),
                        Publisher = c.String(nullable: false, maxLength: 40),
                        PublishDate = c.DateTime(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Pages = c.Int(nullable: false),
                        AvailableCopies = c.Int(nullable: false),
                        ShelfNumber = c.Int(nullable: false),
                        ArriveDate = c.DateTime(nullable: false),
                        Image = c.Binary(),
                        CID = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Categories", t => t.CID, cascadeDelete: true)
                .Index(t => t.CID);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.BookStatus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.EmpAdminUsers",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Salary = c.Int(nullable: false),
                        HireDate = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Body = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            Sql("insert into AspNetRoles values(1,'BaseAdmin') ");
            Sql("insert into AspNetRoles values(2,'Admin') ");
            Sql("insert into AspNetRoles values(3,'Employee') ");
            Sql("insert into AspNetRoles values(4,'Member') ");
            Sql("insert into BookStatus values(1,'Read')");
            Sql("insert into BookStatus values(2,'Reading')");
            Sql("insert into BookStatus values(3,'Returned')");
            Sql("insert into BookStatus values(4,'Borrowing')");




        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.EmpAdminUsers", "ID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Activities", "Status", "dbo.BookStatus");
            DropForeignKey("dbo.Activities", "BookID", "dbo.Books");
            DropForeignKey("dbo.Books", "CID", "dbo.Categories");
            DropForeignKey("dbo.Activities", "MemberID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Notifications", new[] { "UserId" });
            DropIndex("dbo.EmpAdminUsers", new[] { "ID" });
            DropIndex("dbo.Books", new[] { "CID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Activities", new[] { "Status" });
            DropIndex("dbo.Activities", new[] { "BookID" });
            DropIndex("dbo.Activities", new[] { "MemberID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Notifications");
            DropTable("dbo.EmpAdminUsers");
            DropTable("dbo.BookStatus");
            DropTable("dbo.Categories");
            DropTable("dbo.Books");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Activities");
        }
    }
}
