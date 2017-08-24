namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressOfPartners_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddressOfPartners",
                c => new
                    {
                        PartnerId = c.String(nullable: false, maxLength: 7),
                        AddressId = c.String(nullable: false, maxLength: 5),
                        PointId = c.Int(),
                        Address = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => new { t.PartnerId, t.AddressId })
                .ForeignKey("dbo.Partners", t => t.PartnerId, cascadeDelete: true)
                .ForeignKey("dbo.PartnerPoints", t => t.PointId)
                .Index(t => t.PartnerId)
                .Index(t => t.PointId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AddressOfPartners", "PointId", "dbo.PartnerPoints");
            DropForeignKey("dbo.AddressOfPartners", "PartnerId", "dbo.Partners");
            DropIndex("dbo.AddressOfPartners", new[] { "PointId" });
            DropIndex("dbo.AddressOfPartners", new[] { "PartnerId" });
            DropTable("dbo.AddressOfPartners");
        }
    }
}
