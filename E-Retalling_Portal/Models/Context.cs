using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace E_Retalling_Portal.Models
{
    public class Context:DbContext
    {
        private static bool _initialized = false;
        private static bool _resetDb = true;
        private static String? _connectionString= null;
        public DbSet<Account> Accounts {  get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Role> Roles { get; set; }

        static Context()
        {
            InitializeFromXml("configDatabase.xml");
        }
        private static void InitializeFromXml(string xmlFilePath)
        {
            XDocument doc = XDocument.Load(xmlFilePath);

            _connectionString = doc.Root.Element("ConnectionString")?.Value;
            _resetDb = bool.Parse(doc.Root.Element("ResetDb")?.Value);
        }


        public Context()
        {
            if (_resetDb && !_initialized)
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
                _initialized = true;
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
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

            modelBuilder.Entity<Role>()
                .HasData(new Role { id = 1, roleName = "customer" });

        }
    }
}
