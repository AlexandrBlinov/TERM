namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationForUsersUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotificationsForUsers", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.NotificationsForUsers", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NotificationsForUsers", "Status");
            DropColumn("dbo.NotificationsForUsers", "Date");
        }
    }
}
