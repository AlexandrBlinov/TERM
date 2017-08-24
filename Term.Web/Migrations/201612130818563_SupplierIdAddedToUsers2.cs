namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SupplierIdAddedToUsers2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "SupplierId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "SupplierId");
        }
    }
}
