namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaleReturn_SetSaleNumberToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SaleReturnDetails", "SaleNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SaleReturnDetails", "SaleNumber", c => c.Int(nullable: false));
        }
    }
}
