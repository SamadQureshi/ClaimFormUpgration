using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Onion.Domain.Models;
using Onion.Interfaces;
using TrackerEnabledDbContext.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;

namespace Onion.Data
{
    
    public class ApplicationDBContext : TrackerIdentityContext<IdentityUser>, IApplicationDBContext
    {
       public ApplicationDBContext() : base("name=ApplicationConnectionString") {        


        }
       

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        public IDbSet<Department> Department { get; set; }
        public  IDbSet<OpdExpense> OpdExpense { get; set; }
        public  IDbSet<OpdExpenseImage> OpdExpenseImage { get; set; }
        public  IDbSet<OpdExpensePatient> OpdExpensePatient { get; set; }
        public  IDbSet<RelationShipEmployee> RelationShipEmployee { get; set; }

        public IDbSet<TravelExpense> TravelExpense { get; set; }

        public IDbSet<ExpenseType> ExpenseType { get; set; }

        public IDbSet<SetupExpenseAmount> SetupExpenseAmount { get; set; }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public void SaveChanges(string username)
        {    
          
            base.ConfigureMetadata(metadata =>
            {
                metadata.IpAddress = HttpContext.Current.Request.UserHostAddress;  
                
                //metadata.RequestDevice = "AndroidPhone";
                //metadata.Country = Request.Cookies["country"];
            });


            base.SaveChanges(username);

        }

       
    }
}
