namespace ProjectMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstlogin1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FirstLogin", c => c.Boolean(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FirstLogin");
        }
    }
}
