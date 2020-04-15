namespace Onion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TravelExpense : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TravelExpenses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ExpenseType = c.String(maxLength: 100),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        Description = c.String(),
                        OPDEXPENSE_ID = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Created_By = c.String(),
                        Modified_By = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OpdExpenses", t => t.OPDEXPENSE_ID, cascadeDelete: true)
                .Index(t => t.OPDEXPENSE_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TravelExpenses", "OPDEXPENSE_ID", "dbo.OpdExpenses");
            DropIndex("dbo.TravelExpenses", new[] { "OPDEXPENSE_ID" });
            DropTable("dbo.TravelExpenses");
        }
    }
}
