namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Price2_addedToPriceOfPartner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PriceOfPartners", "Price2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Partners", "Sale");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Partners", "Sale", c => c.String());
            DropColumn("dbo.PriceOfPartners", "Price2");
        }
    }
}
