namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropsToUserProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsSupplier", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Users", "IsSaleRep", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "IsSaleRep");
            DropColumn("dbo.Users", "IsSupplier");
        }
    }
}
