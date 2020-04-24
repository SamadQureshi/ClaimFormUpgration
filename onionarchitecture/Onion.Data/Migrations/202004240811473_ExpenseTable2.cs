namespace Onion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpenseTable2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpenseTypes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ExpenseName = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExpenseTypes");
        }
    }
}
