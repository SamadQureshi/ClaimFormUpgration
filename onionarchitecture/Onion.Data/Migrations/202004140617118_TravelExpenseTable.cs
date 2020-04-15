namespace Onion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TravelExpenseTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OpdExpenses", "ManagerName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OpdExpenses", "ManagerName");
        }
    }
}
