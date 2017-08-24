namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManagersOfPartners_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ManagersOfPartners",
                c => new
                    {
                        PartnerId = c.String(nullable: false, maxLength: 7),
                        ManagerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.PartnerId, t.ManagerId })
                .ForeignKey("dbo.Managers", t => t.ManagerId, cascadeDelete: true)
                .ForeignKey("dbo.Partners", t => t.PartnerId, cascadeDelete: true)
                .Index(t => t.PartnerId)
                .Index(t => t.ManagerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ManagersOfPartners", "PartnerId", "dbo.Partners");
            DropForeignKey("dbo.ManagersOfPartners", "ManagerId", "dbo.Managers");
            DropIndex("dbo.ManagersOfPartners", new[] { "ManagerId" });
            DropIndex("dbo.ManagersOfPartners", new[] { "PartnerId" });
            DropTable("dbo.ManagersOfPartners");
        }
    }
}
