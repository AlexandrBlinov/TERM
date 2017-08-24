namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartnerPropertiesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PartnerPropertyDescriptions",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.PartnerPropertyValues",
                c => new
                    {
                        PartnerId = c.String(nullable: false, maxLength: 7),
                        Name = c.String(nullable: false, maxLength: 50),
                        Value = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => new { t.PartnerId, t.Name })
                .ForeignKey("dbo.PartnerPropertyDescriptions", t => t.Name, cascadeDelete: true)
                .Index(t => t.Name);
            
            CreateTable(
                "dbo.ProducerNotDisplayedFromPartners",
                c => new
                    {
                        ProducerId = c.Int(nullable: false),
                        PartnerId = c.String(nullable: false, maxLength: 7),
                    })
                .PrimaryKey(t => new { t.ProducerId, t.PartnerId })
                .ForeignKey("dbo.Partners", t => t.PartnerId, cascadeDelete: true)
                .ForeignKey("dbo.Producers", t => t.ProducerId, cascadeDelete: true)
                .Index(t => t.ProducerId)
                .Index(t => t.PartnerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProducerNotDisplayedFromPartners", "ProducerId", "dbo.Producers");
            DropForeignKey("dbo.ProducerNotDisplayedFromPartners", "PartnerId", "dbo.Partners");
            DropForeignKey("dbo.PartnerPropertyValues", "Name", "dbo.PartnerPropertyDescriptions");
            DropIndex("dbo.ProducerNotDisplayedFromPartners", new[] { "PartnerId" });
            DropIndex("dbo.ProducerNotDisplayedFromPartners", new[] { "ProducerId" });
            DropIndex("dbo.PartnerPropertyValues", new[] { "Name" });
            DropTable("dbo.ProducerNotDisplayedFromPartners");
            DropTable("dbo.PartnerPropertyValues");
            DropTable("dbo.PartnerPropertyDescriptions");
        }
    }
}
