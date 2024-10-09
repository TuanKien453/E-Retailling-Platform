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
                .WithMany(p=>p.orderItems)
                .HasForeignKey(oi=>oi.productItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Image>()
                .HasOne(i=>i.productCovered)
                .WithOne(p=>p.coverImage)
                .HasForeignKey<Image>(i=>i.productCoveredId);

            SeedingCategory(modelBuilder);
            SeedingRole(modelBuilder);
            SeedingUser(modelBuilder);
            SeedingAccount(modelBuilder);
            SeedingStatus(modelBuilder);
            SeedingShop(modelBuilder);
            SeedingImage(modelBuilder);
            SeedingProduct(modelBuilder);
            SeedingProductItem(modelBuilder);

        }
        private static void SeedingShop(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shop>().HasData(
                new Shop {id=1,accountId=3,address="address",name="shopname",createdAt="2000-05-04",shopDescription="sd",statusId=1 }
             );
        }
        private static void SeedingStatus(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Status>().HasData(
                new Status { id=1,statusName="active"}
             );
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
                new Account { id = 2, username = "anh", password = "123", roleId = 1, externalId = null, externalType = null, userId = 2 },
                new Account { id = 3, username = "seller", password = "123", roleId = 2, externalId = null, externalType = null, userId = 2 }
            );
        }

        private static void SeedingImage(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Image>().HasData(
                new Image { id = 1, productId = 1, imageName = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRrR_VvLy3HYbsqzU7IKn8M5CQhguNszaK1pQ&s" },
                new Image { id = 2, productId = 1, imageName = "https://static.nike.com/a/images/t_PDP_936_v1/f_auto,q_auto:eco/450ed1df-8e17-4d87-a244-85697874661c/NIKE+REVOLUTION+7.png" },

                new Image { id = 3, productId = 2, imageName = "https://product.hstatic.net/200000378371/product/_uog2187_829ce01e830942949271a3787398632b_master.jpg" },
                new Image { id = 4, productId = 2, imageName = "https://product.hstatic.net/1000312752/product/atst747-2v-a_aa40e06550894efb82d4d066ea16b687.jpg" },

                new Image { id = 5, productId = 3, imageName = "https://m.media-amazon.com/images/I/81T-W+2GShL._AC_SL1500_.jpg" },
                new Image { id = 6, productId = 3, imageName = "https://m.media-amazon.com/images/I/714t39lASrL._AC_UY1100_.jpg" },

                new Image { id = 7, productId = 4, imageName = "https://www.gap.com/webcontent/0056/081/447/cn56081447.jpg" },
                new Image { id = 8, productId = 4, imageName = "https://product.hstatic.net/200000440297/product/sunday_jeans_1_dbbdb8e06e374c798e470a042a3971f8_master.jpg" },

                new Image { id = 9, productId = 5, imageName = "https://tummachines.com/wp-content/uploads/2023/11/bomber-beige-1.jpg" },
                new Image { id = 10, productId = 5, imageName = "https://cdn.shopify.com/s/files/1/0123/5065/2473/files/BM17064.473BLK_BLACK-STORM-STOPPER-BOMBER-JACKET.jpg?v=1696607398" },

                new Image { id = 11, productId = 6, imageName = "https://cdn.viettelstore.vn/Images/Product/ProductImage/594842402.jpeg" },
                new Image { id = 12, productId = 6, imageName = "https://m.media-amazon.com/images/I/61icsCcbdKL.jpg" },

                new Image { id = 13, productId = 7, imageName = "https://m.media-amazon.com/images/I/61pz4pLftaL._AC_UY1000_.jpg" },
                new Image { id = 14, productId = 7, imageName = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTZtBbaz34tgMvMivO0N8H2CK734QXj7qJ8PQ&s" }

             );
        }

        private static void SeedingProduct(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product { id = 1, categoryId = 1, name = "Shoe", shopId = 1, desc = "this is a good product", price = 10, quantity = 10, isVariation = true, status = 1 },
                new Product { id = 2, categoryId = 2, name = "T-shirt", shopId = 1, desc = "High-quality cotton T-shirt", price = 15, quantity = 50, isVariation = false, status = 1 },
                new Product { id = 3, categoryId = 1, name = "Hat", shopId = 1, desc = "Stylish summer hat", price = 8, quantity = 30, isVariation = false, status = 1 },
                new Product { id = 4, categoryId = 3, name = "Jeans", shopId = 1, desc = "Comfortable blue jeans", price = 20, quantity = 25, isVariation = true, status = 1 },
                new Product { id = 5, categoryId = 4, name = "Jacket", shopId = 1, desc = "Warm winter jacket", price = 50, quantity = 15, isVariation = false, status = 1 },
                new Product { id = 6, categoryId = 2, name = "Watch", shopId = 1, desc = "Elegant wristwatch", price = 100, quantity = 5, isVariation = false, status = 1 },
                new Product { id = 7, categoryId = 3, name = "Backpack", shopId = 1, desc = "Durable outdoor backpack", price = 30, quantity = 20, isVariation = true, status = 1 }
);
        }

        private static void SeedingProductItem(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductItem>().HasData(
              new ProductItem { id = 1, productId = 1, quantity = 10, price = 20, rating = 3, imageId = 1, attribute = "size xl" },
              new ProductItem { id = 2, productId = 1, quantity = 15, price = 15, rating = 4, imageId = 2, attribute = "size l" }
);
        }


    }
}
