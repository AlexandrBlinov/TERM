namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HasOwnRestAddedToPartners : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partners", "HasOwnRest", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Partners", "HasOwnRest");
        }
    }
}
