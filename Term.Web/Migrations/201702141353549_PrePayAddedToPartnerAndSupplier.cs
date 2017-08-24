namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrePayAddedToPartnerAndSupplier : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        LanguageName = c.String(nullable: false, maxLength: 50),
                        LanguageDescription = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.LanguageName);
            
            AddColumn("dbo.Partners", "PrePay", c => c.Boolean(nullable: false));
            AddColumn("dbo.Suppliers", "PrePay", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suppliers", "PrePay");
            DropColumn("dbo.Partners", "PrePay");
            DropTable("dbo.Languages");
        }
    }
}
