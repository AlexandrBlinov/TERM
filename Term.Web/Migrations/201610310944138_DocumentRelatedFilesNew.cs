namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentRelatedFilesNew : DbMigration
    {
        public override void Up()
        {
        /*    DropForeignKey("dbo.OrderDetails", "GuidIn1S", "dbo.Orders");
            RenameColumn(table: "dbo.OrderDetails", name: "Order_guid", newName: "GuidIn1S");
            RenameIndex(table: "dbo.OrderDetails", name: "IX_Order_guid", newName: "IX_GuidIn1S");
            DropPrimaryKey("dbo.Orders"); */
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        ContentType = c.String(maxLength: 100),
                        Content = c.Binary(),
                        GuidIn1S = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FilesForDocuments", t => t.GuidIn1S, cascadeDelete: true)
                .Index(t => t.GuidIn1S);
            
            CreateTable(
                "dbo.FilesForDocuments",
                c => new
                    {
                        GuidIn1S = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.GuidIn1S);
            
     /*       AddPrimaryKey("dbo.Orders", "GuidIn1S");
            AddForeignKey("dbo.OrderDetails", "GuidIn1S", "dbo.Orders", "GuidIn1S", cascadeDelete: true);
            DropColumn("dbo.Orders", "Order_guid"); */
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "Order_guid", c => c.Guid(nullable: false));
            DropForeignKey("dbo.OrderDetails", "GuidIn1S", "dbo.Orders");
            DropForeignKey("dbo.Files", "GuidIn1S", "dbo.FilesForDocuments");
            DropIndex("dbo.Files", new[] { "GuidIn1S" });
            DropPrimaryKey("dbo.Orders");
            DropTable("dbo.FilesForDocuments");
            DropTable("dbo.Files");
            AddPrimaryKey("dbo.Orders", "Order_guid");
            RenameIndex(table: "dbo.OrderDetails", name: "IX_GuidIn1S", newName: "IX_Order_guid");
            RenameColumn(table: "dbo.OrderDetails", name: "GuidIn1S", newName: "Order_guid");
            AddForeignKey("dbo.OrderDetails", "GuidIn1S", "dbo.Orders", "GuidIn1S", cascadeDelete: true);
        }
    }
}
