namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationForUsers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationsForUsers",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserName = c.String(maxLength: 50),
                        Message = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Suppliers", "Name", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suppliers", "Name");
            DropTable("dbo.NotificationsForUsers");
        }
    }
}
