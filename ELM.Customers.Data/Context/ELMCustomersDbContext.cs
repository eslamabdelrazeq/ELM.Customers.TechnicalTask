using ELM.Common;
using ELM.Common.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM.Customers.Database.Context
{
    public class ELMCustomersDbContext : DbContext
    {
        public ELMCustomersDbContext(DbContextOptions options) : base(options)
        {

        }
        public ELMCustomersDbContext()
        {
        }

        #region Entities
        public DbSet<Customer> Customers { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string currentConnectionString = SystemConstants.ConnectionString;
                optionsBuilder.UseSqlServer(currentConnectionString);
            }
        }

        public void InitializeSeeding()
        {
            this.Database.EnsureCreated();
        }
    }
}
