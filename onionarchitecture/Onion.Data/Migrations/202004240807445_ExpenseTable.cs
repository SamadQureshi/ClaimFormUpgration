namespace Onion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpenseTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OpdExpenses", "PhysicalDocumentReceived", c => c.Boolean());
            AddColumn("dbo.OpdExpenses", "PayRollMonth", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OpdExpenses", "PayRollMonth");
            DropColumn("dbo.OpdExpenses", "PhysicalDocumentReceived");
        }
    }
}
