using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace E_Retalling_Portal.Models
{
    public class AppContext:DbContext
    {
        private static bool _init = false;
        private static bool resetDb = true;

        public AppContext()
        {
            if (_init == false)
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
                _init = true;
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=ADMIN;Initial Catalog=E-Retalling_Portal;User ID=sa;Password=123456;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
