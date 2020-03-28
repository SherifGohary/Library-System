namespace ProjectMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class two : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "BirthDate");
            AddColumn("dbo.AspNetUsers", "BirthDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Age", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "BirthDate");
        }
    }
}
