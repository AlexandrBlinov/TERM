namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSaleOne : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Sales", "DepartmentId", c => c.Int(nullable:false,defaultValue: 5)); 
            AlterColumn("Sales", "GuidOfOrderIn1S", c => c.Guid(nullable:true)); 
              CreateIndex(table: "SaleDetails",
              columns: new[] { "SaleGuidIn1S" },
              unique: false,
              name: "IX_SaleGuidIn1S");
            CreateIndex(table: "Sales",
              columns: new[] { "GuidOfOrderIn1S" },
              unique: false,
              name: "IX_OrderGuidIn1S");
        }
        
        public override void Down()
        {
            DropIndex(table: "SaleGuidIn1S",
                   name: "IX_SaleGuidIn1S");
            DropIndex(table: "Sales",
                 name: "IX_OrderGuidIn1S");
        }
    }
}
