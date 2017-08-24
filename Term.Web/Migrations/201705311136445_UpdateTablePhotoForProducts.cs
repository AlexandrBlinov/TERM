namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTablePhotoForProducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PhotoForProducts", "NamePhoto", c => c.String(maxLength: 50));
            DropColumn("dbo.PhotoForProducts", "ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PhotoForProducts", "ProductId", c => c.Int(nullable: false));
            DropColumn("dbo.PhotoForProducts", "NamePhoto");
        }
    }
}
