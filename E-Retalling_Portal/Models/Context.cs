using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace E_Retalling_Portal.Models
{
    public class Context : DbContext
    {
        private static bool _initialized = false;
        private static bool _resetDb = true;
        private static String? _connectionString= null;
        public DbSet<Account> Accounts {  get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<Attribute_type> AttributeTypes { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Order_Item> Order_Items { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Product_item> Product_items { get; set; }
        public DbSet<Product_option> Product_options { get; set; }
        public DbSet<Shipment> Shipments  { get; set; }
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
                .HasForeignKey(s => s.account_id);

                modelBuilder.Entity<Product>()
                .HasOne(s => s.shop)
                .WithMany(s => s.products)
                .HasForeignKey(p => p.shopId);

            modelBuilder.Entity<Product>()
                .HasOne(c => c.category)
                .WithMany(c => c.products)
                .HasForeignKey(p => p.categoryId);

            modelBuilder.Entity<Attribute_type>()
                .HasOne(p => p.product)
                .WithMany(p => p.attribute_types)
                .HasForeignKey(a_t => a_t.product_id);

            modelBuilder.Entity<Attribute>()
                .HasOne(a_t => a_t.attributeType)
                .WithMany(a_t => a_t.attributes)
                .HasForeignKey(a => a.attributeTypeId);

            modelBuilder.Entity<Product_option>()
                .HasOne(a => a.attribute)
                .WithMany(a => a.product_Options)
                .HasForeignKey(p_o => p_o.attribute_id);

            modelBuilder.Entity<Product_option>()
                .HasOne(p_i => p_i.product_Item)
                .WithMany(p_i => p_i.product_Options)
                .HasForeignKey(p_o => p_o.product_item_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product_item>()
                .HasOne(p => p.product)
                .WithMany(p => p.product_Items)
                .HasForeignKey(p_i => p_i.productId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product_item>()
                .HasOne(i => i.image)
                .WithMany(i => i.product_items)
                .HasForeignKey( p_i => p_i.image_id);

            modelBuilder.Entity<Image>()
                .HasOne(p => p.product)
                .WithMany(p => p.images)
                .HasForeignKey(i => i.product_id);

            modelBuilder.Entity<Order>()
                .HasOne(u => u.user)
                .WithMany(u => u.orders)
                .HasForeignKey(o => o.user_id);

            modelBuilder.Entity<Order_Item>()
                .HasOne(o => o.order)
                .WithMany(o => o.order_Items)
                .HasForeignKey(o_i => o_i.order_id);


            modelBuilder.Entity<Order_Item>()
                .HasOne(p => p.product)
                .WithMany(p => p.order_Items)
                .HasForeignKey(o_i => o_i.product_Item_Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shipment>()
                .HasOne(o_i => o_i.order_Item)
                .WithMany(o_i => o_i.shipments)
                .HasForeignKey(s => s.order_Item_Id);

            modelBuilder.Entity<Product_option>()
                .HasKey(p_o => new { p_o.product_item_id, p_o.attribute_id });

            base.OnModelCreating(modelBuilder);

            SeedingCategory(modelBuilder);
            SeedingRole(modelBuilder);
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
    }
}
