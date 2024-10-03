using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace E_Retalling_Portal.Models
{
    public class Context : DbContext
    {
        private static bool _initialized = false;
        private static bool _resetDb = true;
        private static String? _connectionString = null;
        public DbSet<Account> Accounts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<AttributeType> AttributeTypes { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Status> Statuses { get; set; }



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

            modelBuilder.Entity<Category>()
                .HasOne(c => c.parent)
                .WithMany(c => c.childrens)
                .HasForeignKey(c => c.parentCategoryId);

            modelBuilder.Entity<Shop>()
                .HasOne(st => st.status)
                .WithMany(st => st.shops)
                .HasForeignKey(s => s.statusId);

            modelBuilder.Entity<Shop>()
                .HasOne(a => a.account)
                .WithMany(a => a.shops)
                .HasForeignKey(s => s.accountId);

            modelBuilder.Entity<Product>()
            .HasOne(s => s.shop)
            .WithMany(s => s.products)
            .HasForeignKey(p => p.shopId);

            modelBuilder.Entity<Product>()
                .HasOne(c => c.category)
                .WithMany(c => c.products)
                .HasForeignKey(p => p.categoryId);

            modelBuilder.Entity<AttributeType>()
                .HasOne(p => p.product)
                .WithMany(p => p.attributeTypes)
                .HasForeignKey(a_t => a_t.productId);

            modelBuilder.Entity<Attribute>()
                .HasOne(a_t => a_t.attributeType)
                .WithMany(a_t => a_t.attributes)
                .HasForeignKey(a => a.attributeTypeId);

            modelBuilder.Entity<ProductOption>()
                .HasOne(a => a.attribute)
                .WithMany(a => a.productOptions)
                .HasForeignKey(p_o => p_o.attributeId);

            modelBuilder.Entity<ProductOption>()
                .HasOne(p_i => p_i.productItem)
                .WithMany(p_i => p_i.productOptions)
                .HasForeignKey(p_o => p_o.productItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductItem>()
                .HasOne(p => p.product)
                .WithMany(p => p.productItems)
                .HasForeignKey(p_i => p_i.productId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductItem>()
                .HasOne(i => i.image)
                .WithOne(i => i.productItem)
                .HasForeignKey<ProductItem>(p_i => p_i.imageId);

            modelBuilder.Entity<Image>()
                .HasOne(p => p.product)
                .WithMany(p => p.images)
                .HasForeignKey(i => i.productId);

            modelBuilder.Entity<Order>()
                .HasOne(u => u.user)
                .WithMany(u => u.orders)
                .HasForeignKey(o => o.userId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.order)
                .WithMany(o => o.orderItems)
                .HasForeignKey(o_i => o_i.orderId);


            modelBuilder.Entity<OrderItem>()
                .HasOne(p => p.productItem)
                .WithOne(p=>p.orderItem)
                .HasForeignKey<OrderItem>(oi=>oi.productItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductOption>()
                .HasKey(p_o => new { p_o.productItemId, p_o.attributeId });


            SeedingCategory(modelBuilder);
            SeedingRole(modelBuilder);
            SeedingUser(modelBuilder);
            SeedingAccount(modelBuilder);
        }

        private static void SeedingCategory(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { id = 1, name = "Categories 1", parentCategoryId = null },
                new Category { id = 2, name = "Categories 2", parentCategoryId = null },
                new Category { id = 3, name = "Subcategory 1", parentCategoryId = 1 },
                new Category { id = 4, name = "Subcategory 2", parentCategoryId = 3 },
                new Category { id = 5, name = "Subcategory 3", parentCategoryId = 2 }
             );
        }
        private static void SeedingRole(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
            new Role { id = 1, roleName = "Customer" },
            new Role { id = 2, roleName = "Shop Owner" },
            new Role { id = 3, roleName = "Manager" }
            );
        }

        private static void SeedingUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    id = 1,
                    address = "address",
                    birthday = "2000-05-04",
                    displayName = "kienhocgioi",
                    email = "abc@gmail.com",
                    firstName = "first",
                    lastName = "last",
                    phoneNumber = "0123456789",
                    gender = "Female"
                },
                new User
                {
                    id = 2,
                    address = "addresdds",
                    birthday = "2000-01-04",
                    displayName = "anh",
                    email = "abcadsf@gmail.com",
                    firstName = "first",
                    lastName = "last",
                    phoneNumber = "0123459145",
                    gender = "Female"
                }

            );
        }

        private static void SeedingAccount(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasData(
                new Account { id = 1, username = "admin", password = "123", roleId = 1, externalId = null, externalType = null, userId = 1 },
                new Account { id = 2, username = "anh", password = "123", roleId = 1, externalId = null, externalType = null, userId = 2 }
            );
        }
    }
}
