namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDropIndexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex(table: "Producers",
              columns: new[] { "Name" },
              unique: false,
              name: "IX_ProducerName");

            CreateIndex(table: "Products",
             columns: new[] { "Name" },
             unique: false,
             name: "IX_ProductName");

            CreateIndex(table: "Orders",
              columns: new[] { "GuidIn1S" },
              unique: false,
              name: "IX_GuidIn1S");



        }

        public override void Down()
        {
            DropIndex(table: "Producers",
                  name: "IX_ProducerName");
            DropIndex(table: "Products",
                 name: "IX_ProductName");
            DropIndex(table: "Orders",
               name: "IX_GuidIn1S");

        }
    }
}
