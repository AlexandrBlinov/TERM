namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartnerPointsEmailAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PartnerPoints", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PartnerPoints", "Email");
        }
    }
}
