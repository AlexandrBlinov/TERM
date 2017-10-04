namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PriceIsPrepay2_AddedTocart : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "PriceIsPrepay", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carts", "PriceIsPrepay");
        }
    }
}
