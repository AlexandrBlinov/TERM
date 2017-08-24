namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaleReturnAddedDocDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SaleReturns", "DocDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SaleReturns", "DocDate");
        }
    }
}
