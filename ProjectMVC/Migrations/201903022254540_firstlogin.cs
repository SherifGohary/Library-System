namespace ProjectMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstlogin : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "FirstLogin");
        }

        public override void Down()
        {
        }
    }
}
