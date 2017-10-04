namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HasPrepay_AddedToPartners : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partners", "HasStar", c => c.Boolean(nullable: false));
            AddColumn("dbo.Partners", "HasPrepay", c => c.Boolean(nullable: false));
            AddColumn("dbo.Partners", "WayOfDelivery", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Partners", "WayOfDelivery");
            DropColumn("dbo.Partners", "HasPrepay");
            DropColumn("dbo.Partners", "HasStar");
        }
    }
}
