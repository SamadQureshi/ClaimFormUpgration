namespace Onion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTable2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Created_By = c.String(),
                        Modified_By = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.OpdExpense_Image",
                c => new
                    {
                        IMAGE_ID = c.Int(nullable: false, identity: true),
                        OPDEXPENSE_ID = c.Int(),
                        IMAGE_NAME = c.String(),
                        IMAGE_EXT = c.String(),
                        IMAGE_BASE64 = c.String(),
                        NAME_EXPENSES = c.String(),
                        EXPENSE_AMOUNT = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Created_By = c.String(),
                        Modified_By = c.String(),
                    })
                .PrimaryKey(t => t.IMAGE_ID)
                .ForeignKey("dbo.OpdExpenses", t => t.OPDEXPENSE_ID)
                .Index(t => t.OPDEXPENSE_ID);
            
            CreateTable(
                "dbo.OpdExpenses",
                c => new
                    {
                        OPDEXPENSE_ID = c.Int(nullable: false, identity: true),
                        EMPLOYEE_EMAILADDRESS = c.String(maxLength: 100),
                        EMPLOYEE_NAME = c.String(maxLength: 100),
                        EMPLOYEE_DEPARTMENT = c.String(maxLength: 100),
                        CLAIM_MONTH = c.String(maxLength: 50),
                        CLAIM_YEAR = c.String(maxLength: 50),
                        TOTAL_AMOUNT_CLAIMED = c.Decimal(precision: 18, scale: 2),
                        HR_COMMENT = c.String(),
                        HR_APPROVAL = c.Boolean(),
                        HR_APPROVAL_DATE = c.DateTime(),
                        HR_NAME = c.String(maxLength: 100),
                        FINANCE_COMMENT = c.String(),
                        FINANCE_APPROVAL = c.Boolean(),
                        FINANCE_APPROVAL_DATE = c.DateTime(),
                        FINANCE_NAME = c.String(maxLength: 100),
                        MANAGEMENT_COMMENT = c.String(),
                        MANAGEMENT_APPROVAL = c.Boolean(),
                        MANAGEMENT_APPROVAL_DATE = c.DateTime(),
                        MANAGEMENT_NAME = c.String(maxLength: 100),
                        DATE_ILLNESS_NOTICED = c.DateTime(),
                        DATE_RECOVERY = c.DateTime(),
                        DIAGNOSIS = c.String(),
                        CLAIMANT_SUFFERED_ILLNESS = c.Boolean(),
                        CLAIMANT_SUFFERED_ILLNESS_DATE = c.DateTime(),
                        CLAIMANT_SUFFERED_ILLNESS_DETAILS = c.String(),
                        HOSPITAL_NAME = c.String(maxLength: 500),
                        DOCTOR_NAME = c.String(maxLength: 500),
                        PERIOD_CONFINEMENT_DATE_FROM = c.DateTime(),
                        PERIOD_CONFINEMENT_DATE_TO = c.DateTime(),
                        DRUGS_PRESCRIBED_BOOL = c.Boolean(),
                        DRUGS_PRESCRIBED_DESCRIPTION = c.String(),
                        OPDTYPE = c.String(maxLength: 100),
                        HR_EMAILADDRESS = c.String(maxLength: 100),
                        FINANCE_EMAILADDRESS = c.String(maxLength: 100),
                        MANAGEMENT_EMAILADDRESS = c.String(maxLength: 100),
                        STATUS = c.String(maxLength: 100),
                        TOTAL_AMOUNT_APPROVED = c.Decimal(precision: 18, scale: 2),
                        EXPENSE_NUMBER = c.String(maxLength: 4000),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Created_By = c.String(),
                        Modified_By = c.String(),
                    })
                .PrimaryKey(t => t.OPDEXPENSE_ID);
            
            CreateTable(
                "dbo.OpdExpense_Patient",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NAME = c.String(maxLength: 100),
                        AGE = c.Int(),
                        RELATIONSHIP_EMPLOYEE = c.String(nullable: false, maxLength: 50),
                        OPDEXPENSE_ID = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Created_By = c.String(),
                        Modified_By = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OpdExpenses", t => t.OPDEXPENSE_ID, cascadeDelete: true)
                .Index(t => t.OPDEXPENSE_ID);
            
            CreateTable(
                "dbo.RelationShip_Employee",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RELATIONSHIP = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Created_By = c.String(),
                        Modified_By = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Created_By = c.String(),
                        Modified_By = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OpdExpense_Patient", "OPDEXPENSE_ID", "dbo.OpdExpenses");
            DropForeignKey("dbo.OpdExpense_Image", "OPDEXPENSE_ID", "dbo.OpdExpenses");
            DropIndex("dbo.OpdExpense_Patient", new[] { "OPDEXPENSE_ID" });
            DropIndex("dbo.OpdExpense_Image", new[] { "OPDEXPENSE_ID" });
            DropTable("dbo.Users");
            DropTable("dbo.RelationShip_Employee");
            DropTable("dbo.OpdExpense_Patient");
            DropTable("dbo.OpdExpenses");
            DropTable("dbo.OpdExpense_Image");
            DropTable("dbo.Departments");
        }
    }
}
