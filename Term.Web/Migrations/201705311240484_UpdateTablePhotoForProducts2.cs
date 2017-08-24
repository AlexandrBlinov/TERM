namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTablePhotoForProducts2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PhotoForProducts", "NamePhoto", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PhotoForProducts", "NamePhoto", c => c.String(maxLength: 50));
        }
    }
}
