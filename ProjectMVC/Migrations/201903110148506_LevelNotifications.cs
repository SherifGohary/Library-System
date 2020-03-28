namespace ProjectMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LevelNotifications : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "Level", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "Level");
        }
    }
}
