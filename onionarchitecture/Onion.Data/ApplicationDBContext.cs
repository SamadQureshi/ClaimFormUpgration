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

        public IDbSet<Department> Department { get; set; }
        public  IDbSet<OpdExpense> OpdExpense { get; set; }
        public  IDbSet<OpdExpenseImage> OpdExpenseImage { get; set; }
        public  IDbSet<OpdExpensePatient> OpdExpensePatient { get; set; }
        public  IDbSet<RelationShipEmployee> RelationShipEmployee { get; set; }

        public IDbSet<TravelExpense> TravelExpense { get; set; }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }
    }
}
