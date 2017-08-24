namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RelatedFiles : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Files", "GuidIn1S", "dbo.FilesForDocuments");
            DropIndex("dbo.Files", new[] { "GuidIn1S" });
            DropTable("dbo.FilesForDocuments");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.FilesForDocuments",
                c => new
                    {
                        GuidIn1S = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.GuidIn1S);
            
            CreateIndex("dbo.Files", "GuidIn1S");
            AddForeignKey("dbo.Files", "GuidIn1S", "dbo.FilesForDocuments", "GuidIn1S", cascadeDelete: true);
        }
    }
}
