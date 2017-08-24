namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTablePhotoForProducts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PhotoForProducts",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        NumberPhoto = c.Int(nullable: false),
                        ContentType = c.String(maxLength: 100),
                        Photo = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PhotoForProducts");
        }
    }
}
