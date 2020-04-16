namespace Onion.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TravelExpense : DbMigration
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
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.OpdExpenses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EmployeeEmailAddress = c.String(maxLength: 100),
                        EmployeeName = c.String(maxLength: 100),
                        EmployeeDepartment = c.String(maxLength: 100),
                        OpdType = c.String(maxLength: 100),
                        Status = c.String(maxLength: 100),
                        ClaimMonth = c.String(maxLength: 50),
                        ClaimYear = c.String(maxLength: 50),
                        TotalAmountClaimed = c.Decimal(precision: 18, scale: 2),
                        TotalAmountApproved = c.Decimal(precision: 18, scale: 2),
                        HrComment = c.String(),
                        HrApproval = c.Boolean(),
                        HrApprovalDate = c.DateTime(),
                        HrEmailAddress = c.String(maxLength: 100),
                        HrName = c.String(maxLength: 100),
                        FinanceComment = c.String(),
                        FinanceApproval = c.Boolean(),
                        FinanceApprovalDate = c.DateTime(),
                        FinanceEmailAddress = c.String(maxLength: 100),
                        FinanceName = c.String(maxLength: 100),
                        ManagementComment = c.String(),
                        ManagementApproval = c.Boolean(),
                        ManagementApprovalDate = c.DateTime(),
                        ManagementName = c.String(maxLength: 100),
                        ManagementEmailAddress = c.String(maxLength: 100),
                        DateIllnessNoticed = c.DateTime(),
                        DateRecovery = c.DateTime(),
                        Diagnosis = c.String(),
                        ClaimantSufferedIllness = c.Boolean(),
                        ClaimantSufferedIllnessDate = c.DateTime(),
                        ClaimantSufferedIllnessDetails = c.String(),
                        HospitalName = c.String(maxLength: 500),
                        DoctorName = c.String(maxLength: 500),
                        PeriodConfinementDateFrom = c.DateTime(),
                        PeriodConfinementDateTo = c.DateTime(),
                        DrugsPrescribedBool = c.Boolean(),
                        DrugsPrescribedDescription = c.String(),
                        ManagerName = c.String(maxLength: 100),
                        ExpenseNumber = c.String(maxLength: 4000),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.OpdExpenseImages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ImageName = c.String(),
                        ImageExt = c.String(),
                        ImageBase64 = c.String(),
                        NameExpenses = c.String(),
                        ExpenseAmount = c.Decimal(precision: 18, scale: 2),
                        OpdExpenseId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OpdExpenses", t => t.OpdExpenseId, cascadeDelete: true)
                .Index(t => t.OpdExpenseId);
            
            CreateTable(
                "dbo.OpdExpensePatients",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Age = c.Int(),
                        RelationshipEmployee = c.String(nullable: false, maxLength: 50),
                        OpdExpenseId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OpdExpenses", t => t.OpdExpenseId, cascadeDelete: true)
                .Index(t => t.OpdExpenseId);
            
            CreateTable(
                "dbo.RelationShipEmployees",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RelationShip = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TravelExpenses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ExpenseType = c.String(maxLength: 100),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        Description = c.String(),
                        OpdExpenseId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OpdExpenses", t => t.OpdExpenseId, cascadeDelete: true)
                .Index(t => t.OpdExpenseId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TravelExpenses", "OpdExpenseId", "dbo.OpdExpenses");
            DropForeignKey("dbo.OpdExpensePatients", "OpdExpenseId", "dbo.OpdExpenses");
            DropForeignKey("dbo.OpdExpenseImages", "OpdExpenseId", "dbo.OpdExpenses");
            DropIndex("dbo.TravelExpenses", new[] { "OpdExpenseId" });
            DropIndex("dbo.OpdExpensePatients", new[] { "OpdExpenseId" });
            DropIndex("dbo.OpdExpenseImages", new[] { "OpdExpenseId" });
            DropTable("dbo.Users");
            DropTable("dbo.TravelExpenses");
            DropTable("dbo.RelationShipEmployees");
            DropTable("dbo.OpdExpensePatients");
            DropTable("dbo.OpdExpenseImages");
            DropTable("dbo.OpdExpenses");
            DropTable("dbo.Departments");
        }
    }
}
