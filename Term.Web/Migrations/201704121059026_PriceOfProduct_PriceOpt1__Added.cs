namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PriceOfProduct_PriceOpt1__Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PriceOfProducts", "PriceOpt1", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PriceOfProducts", "PriceOpt1");
        }
    }
}
