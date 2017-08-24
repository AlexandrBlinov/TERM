namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeedbackFormToDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeedbackFormDbSet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        Message = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FeedbackFormDbSet");
        }
    }
}
