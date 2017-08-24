namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaleReturnAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SaleReturnDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GuidIn1S = c.Guid(nullable: false),
                        SaleDate = c.DateTime(nullable: false),
                        SaleNumber = c.Int(nullable: false),
                        RowNumber = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Count = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.SaleReturns", t => t.GuidIn1S, cascadeDelete: true)
                .Index(t => t.GuidIn1S)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.SaleReturns",
                c => new
                    {
                        GuidIn1S = c.Guid(nullable: false),
                        NumberIn1S = c.String(),
                        PartnerId = c.String(maxLength: 7),
                        PointId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GuidIn1S)
                .ForeignKey("dbo.PartnerPoints", t => t.PointId, cascadeDelete: true)
                .Index(t => t.PointId);
            
            AddColumn("dbo.Orders", "DateOfPayment", c => c.DateTime());
            AddColumn("dbo.PartnerPoints", "LatLng", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SaleReturnDetails", "GuidIn1S", "dbo.SaleReturns");
            DropForeignKey("dbo.SaleReturns", "PointId", "dbo.PartnerPoints");
            DropForeignKey("dbo.SaleReturnDetails", "ProductId", "dbo.Products");
            DropIndex("dbo.SaleReturns", new[] { "PointId" });
            DropIndex("dbo.SaleReturnDetails", new[] { "ProductId" });
            DropIndex("dbo.SaleReturnDetails", new[] { "GuidIn1S" });
            DropColumn("dbo.PartnerPoints", "LatLng");
            DropColumn("dbo.Orders", "DateOfPayment");
            DropTable("dbo.SaleReturns");
            DropTable("dbo.SaleReturnDetails");
        }
    }
}
