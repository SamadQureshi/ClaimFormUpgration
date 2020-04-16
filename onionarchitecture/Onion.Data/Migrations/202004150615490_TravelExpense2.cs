namespace Onion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TravelExpense2 : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [OpdExpenses] ADD [ExpenseNumber] AS ((('CF-'+format(getdate(),'MMyyyy'))+replicate('0',(5)-len(CONVERT([nvarchar](50),[ID]))))+CONVERT([nvarchar](50),[ID]))");
        }
        
        public override void Down()
        {
        }
    }
}
