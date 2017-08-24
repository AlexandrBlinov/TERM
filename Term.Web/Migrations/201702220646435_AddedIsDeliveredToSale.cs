namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsDeliveredToSale : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sales", "IsDelivered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sales", "IsDelivered");
        }
    }
}
