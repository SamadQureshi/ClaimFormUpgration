using System.Configuration;
using System.Data.Entity;
using Onion.Domain.Models;
using Onion.Interfaces;

namespace Onion.Data
{
    public class ApplicationDBContext : DbContext, IApplicationDBContext
    {
       public ApplicationDBContext() : base("name=ApplicationConnectionString") { }
        //public ApplicationDBContext(string connectionString = "ApplicationConnectionString")
        //    : base("name=ApplicationConnectionString")
        //{

        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        public IDbSet<User> Users { get; set; }

        public IDbSet<Department> Departments { get; set; }
        public  IDbSet<OpdExpense> OpdExpenses { get; set; }
        public  IDbSet<OpdExpense_Image> OpdExpense_Image { get; set; }
        public  IDbSet<OpdExpense_Patient> OpdExpense_Patient { get; set; }
        public  IDbSet<RelationShip_Employee> RelationShip_Employee { get; set; }

        public IDbSet<TravelExpense> TravelExpense { get; set; }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }
    }
}
