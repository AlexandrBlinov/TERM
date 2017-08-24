namespace YstProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NumberOfDaysForReturnAddedToPartner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partners", "NumberOfDaysForReturn", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Partners", "NumberOfDaysForReturn");
        }
    }
}
