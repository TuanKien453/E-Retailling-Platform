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
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
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
                .WithOne(a => a.shop)
                .HasForeignKey<Shop>(s => s.accountId);

            modelBuilder.Entity<Product>()
            .HasOne(s => s.shop)
            .WithMany(s => s.products)
            .HasForeignKey(p => p.shopId);

            modelBuilder.Entity<Product>()
                .HasOne(c => c.category)
                .WithMany(c => c.products)
                .HasForeignKey(p => p.categoryId);

            modelBuilder.Entity<ProductItem>()
                .HasOne(p => p.product)
                .WithMany(p => p.productItems)
                .HasForeignKey(p_i => p_i.productId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductItem>()
                .HasOne(i => i.image)
                .WithMany(i => i.productItems)
                .HasForeignKey(p_i => p_i.imageId);

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
                .WithMany(p => p.orderItems)
                .HasForeignKey(oi => oi.productItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.productCovered)
                .WithOne(p => p.coverImage)
                .HasForeignKey<Image>(i => i.productCoveredId);

            SeedingCategory(modelBuilder);
            SeedingRole(modelBuilder);
            SeedingUser(modelBuilder);
            SeedingAccount(modelBuilder);
            SeedingStatus(modelBuilder);
            SeedingShop(modelBuilder);
            SeedingProduct(modelBuilder);
            SeedingImage(modelBuilder);
            SeedingProductItem(modelBuilder);
        }
        private static void SeedingProductItem(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductItem>().HasData(
                new ProductItem { id = 1, productId = 1, quantity = 5, price = 15, imageId = 1, attribute = "L" },
                new ProductItem { id = 2, productId = 1, quantity = 7, price = 20, imageId = 2, attribute = "S" },
                new ProductItem { id = 3, productId = 3, quantity = 10, price = 25, imageId = 3, attribute = "X" },
                new ProductItem { id = 4, productId = 3, quantity = 10, price = 25, imageId = 4, attribute = "X" }
            );
        }
        private static void SeedingImage(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Image>().HasData(
                new Image { id = 1, productId = 1, imageName = "Screenshot 2024-09-13 154422.png", productCoveredId = 1 },
                new Image { id = 2, productId = 1, imageName = "Screenshot 2024-09-14 075432.png" },
                new Image { id = 3, productId = 2, imageName = "Screenshot 2024-09-14 075432.png", productCoveredId = 2 },
                new Image { id = 4, productId = 3, imageName = "Screenshot 2024-09-14 080322.png", productCoveredId = 3 },
                new Image { id = 5, productId = 3, imageName = "Screenshot 2024-09-16 214607.png" },
                                new Image { id = 6, productId = 4, imageName = "Screenshot 2024-09-16 214607.png", productCoveredId = 4 }

                             );
        }

        private static void SeedingProduct(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product { id = 1, categoryId = 3, name = "Sport Shoe", shopId = 1, desc = "this is a good product", price = 10, quantity = 10, isVariation = true, status = 1 },
                new Product { id = 2, categoryId = 4, name = "T-shirt", shopId = 1, desc = "High-quality cotton T-shirt", price = 15, quantity = 5, isVariation = false, status = 1 },
                new Product { id = 3, categoryId = 5, name = "Jacket", shopId = 1, desc = "High-quality cotton T-shirt", price = 10, quantity = 6, isVariation = true, status = 1 },
                 new Product { id = 4, categoryId = 6, name = "Jacket", shopId = 1, desc = "High-quality cotton T-shirt", price = 10, quantity = 6, isVariation = true, status = 1 }
);
        }
        private static void SeedingCategory(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { id = 1, name = "Clothes", parentCategoryId = null },
                new Category { id = 2, name = "Watch", parentCategoryId = null },
                new Category { id = 3, name = "Jean", parentCategoryId = 1 },
                new Category { id = 4, name = "Shirt", parentCategoryId = 1 },
                new Category { id = 5, name = "Jacket", parentCategoryId = 3 },
                new Category { id = 6, name = "abc", parentCategoryId = 1 }

             );
        }
        private static void SeedingShop(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shop>().HasData(
                new Shop { id = 1, accountId = 1, address = "address", name = "shopname", createdAt = "2000-05-04", shopDescription = "sd", statusId = 1 }
             );
        }
        private static void SeedingStatus(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Status>().HasData(
                new Status { id = 1, statusName = "active" }
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
                new Account { id = 1, username = "admin", password = "123", roleId = 2, externalId = null, externalType = null, userId = 1 },
                new Account { id = 2, username = "anh", password = "123", roleId = 1, externalId = null, externalType = null, userId = 2 },
                new Account { id = 3, username = "seller", password = "123", roleId = 2, externalId = null, externalType = null, userId = 2 }
            );
        }
    }
}
