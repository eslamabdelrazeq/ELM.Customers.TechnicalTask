using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM.Customers.Database.Context
{
    public class ApplicationContextDbFactory : IDesignTimeDbContextFactory<ELMCustomersDbContext>
    {
        public ApplicationContextDbFactory()
        {
        }
        ELMCustomersDbContext IDesignTimeDbContextFactory<ELMCustomersDbContext>.CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ELMCustomersDbContext> optionsBuilder = new DbContextOptionsBuilder<ELMCustomersDbContext>();
            optionsBuilder.UseSqlServer<ELMCustomersDbContext>("Server=DESKTOP-7SMO040; Database=ELMCustomers; User ID=islam;Password=123456");
            return new ELMCustomersDbContext(optionsBuilder.Options);
        }
    }
}
