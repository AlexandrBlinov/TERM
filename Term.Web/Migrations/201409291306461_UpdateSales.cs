namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSales : DbMigration
    {
        public override void Up()
        {
            AddColumn("Sales", "Url", c => c.String()); 
        }
        
        public override void Down()
        {
        }
    }
}
