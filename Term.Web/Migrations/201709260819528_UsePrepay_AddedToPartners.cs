namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsePrepay_AddedToPartners : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partners", "UsePrepayPrices", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Partners", "UsePrepayPrices");
        }
    }
}
