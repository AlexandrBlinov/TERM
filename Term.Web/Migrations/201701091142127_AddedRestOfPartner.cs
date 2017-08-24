namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRestOfPartner : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RestOfPartners",
                c => new
                    {
                        PartnerId = c.String(nullable: false, maxLength: 7),
                        ProductId = c.Int(nullable: false),
                        Rest = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PartnerId, t.ProductId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RestOfPartners");
        }
    }
}
