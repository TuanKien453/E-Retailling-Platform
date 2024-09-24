using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace E_Retalling_Portal.Models
{
    public class LocalAppContext:DbContext
    {
        private static bool _init = false;
        private static bool resetDb = true;

        public DbSet<Account> Account {  get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Role> Role { get; set; }

        public LocalAppContext()
        {
            if (_init == false && resetDb == true)
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
                _init = true;
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=LAPTOP-EGV9P77B;Initial Catalog=E-Retalling_Portal;User ID=sa;Password=vainglory;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasOne(r => r.role)
                .WithMany(r => r.accounts)
                .HasForeignKey(a => a.roleId);

            modelBuilder.Entity<Account>()
                .HasOne(u => u.user)
                .WithMany(u => u.accounts)
                .HasForeignKey(a => a.userId);

            modelBuilder.Entity<Address>()
                .HasOne(u => u.user)
                .WithMany(u => u.addresses)
                .HasForeignKey(a => a.userId);

        }
    }
}
