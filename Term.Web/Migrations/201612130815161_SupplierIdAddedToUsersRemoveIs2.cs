namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SupplierIdAddedToUsersRemoveIs2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "SupplierId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "SupplierId", c => c.Int());
        }
    }
}
