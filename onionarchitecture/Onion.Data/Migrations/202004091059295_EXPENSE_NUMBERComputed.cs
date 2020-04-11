namespace Onion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EXPENSE_NUMBERComputed : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [OpdExpenses] ADD [EXPENSE_NUMBER] AS ((('CF-'+format(getdate(),'MMyyyy'))+replicate('0',(5)-len(CONVERT([nvarchar](50),[OPDEXPENSE_ID]))))+CONVERT([nvarchar](50),[OPDEXPENSE_ID]))");

        }
        
        public override void Down()
        {
        }
    }
}
