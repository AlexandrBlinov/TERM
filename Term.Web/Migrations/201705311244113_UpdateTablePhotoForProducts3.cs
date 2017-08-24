namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTablePhotoForProducts3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PhotoForProducts", "NamePhoto", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PhotoForProducts", "NamePhoto", c => c.String(maxLength: 100));
        }
    }
}
