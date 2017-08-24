namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeasonStockItemOfPartnerAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SeasonStockItemOfPartners",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        PartnerId = c.String(nullable: false, maxLength: 7),
                    })
                .PrimaryKey(t => new { t.ProductId, t.PartnerId })
                .ForeignKey("dbo.Partners", t => t.PartnerId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.PartnerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SeasonStockItemOfPartners", "ProductId", "dbo.Products");
            DropForeignKey("dbo.SeasonStockItemOfPartners", "PartnerId", "dbo.Partners");
            DropIndex("dbo.SeasonStockItemOfPartners", new[] { "PartnerId" });
            DropIndex("dbo.SeasonStockItemOfPartners", new[] { "ProductId" });
            DropTable("dbo.SeasonStockItemOfPartners");
        }
    }
}
