namespace Onion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TravelExpenseImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TravelExpenses", "ImageName", c => c.String());
            AddColumn("dbo.TravelExpenses", "ImageExt", c => c.String());
            AddColumn("dbo.TravelExpenses", "ImageBase64", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TravelExpenses", "ImageBase64");
            DropColumn("dbo.TravelExpenses", "ImageExt");
            DropColumn("dbo.TravelExpenses", "ImageName");
        }
    }
}
