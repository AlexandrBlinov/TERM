namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManagerContactDataAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Managers", "MobilePhoneNumber", c => c.String(maxLength: 255));
            AddColumn("dbo.Managers", "InternalPhoneNumber", c => c.String(maxLength: 255));
            AddColumn("dbo.Managers", "Skype", c => c.String(maxLength: 100));
            AddColumn("dbo.Managers", "ICQ", c => c.String(maxLength: 100));
            AlterColumn("dbo.Users", "Email", c => c.String(maxLength: 256));
            AlterColumn("dbo.Users", "UserName", c => c.String(nullable: false, maxLength: 256));
            CreateIndex("dbo.Users", "UserName", unique: true, name: "UserNameIndex");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "UserNameIndex");
            AlterColumn("dbo.Users", "UserName", c => c.String());
            AlterColumn("dbo.Users", "Email", c => c.String());
            DropColumn("dbo.Managers", "ICQ");
            DropColumn("dbo.Managers", "Skype");
            DropColumn("dbo.Managers", "InternalPhoneNumber");
            DropColumn("dbo.Managers", "MobilePhoneNumber");
        }
    }
}
