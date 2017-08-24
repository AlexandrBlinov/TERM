namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductPropertyDescription_Added2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ProductProperties", newName: "ProductPropertyValues");
            CreateTable(
                "dbo.ProductPropertyDescriptions",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 50),
                        NameToRus = c.String(maxLength: 50),
                        ProductType = c.Int(),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateIndex("dbo.ProductPropertyValues", "Name");
            AddForeignKey("dbo.ProductPropertyValues", "Name", "dbo.ProductPropertyDescriptions", "Name", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductPropertyValues", "Name", "dbo.ProductPropertyDescriptions");
            DropIndex("dbo.ProductPropertyValues", new[] { "Name" });
            DropTable("dbo.ProductPropertyDescriptions");
            RenameTable(name: "dbo.ProductPropertyValues", newName: "ProductProperties");
        }
    }
}
