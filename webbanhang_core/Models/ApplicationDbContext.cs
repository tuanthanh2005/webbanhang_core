using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace webbanhang_core.Models
{
    // ✅ AppUser tách riêng
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime? Birthday { get; set; }
    }

    // ✅ ApplicationDbContext tách riêng (không lồng trong AppUser)
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Điện thoại", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Máy tính bảng", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Laptop", DisplayOrder = 3 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Iphone 7", Price = 300, CategoryId = 1 },
                new Product { Id = 2, Name = "Iphone 7s", Price = 350, CategoryId = 1 },
                new Product { Id = 3, Name = "Iphone 8", Price = 400, CategoryId = 1 },
                new Product { Id = 4, Name = "Iphone 8s", Price = 420, CategoryId = 1 },
                new Product { Id = 5, Name = "Iphone 12", Price = 630, CategoryId = 1 },
                new Product { Id = 6, Name = "Iphone 12 Pro", Price = 750, CategoryId = 1 },
                new Product { Id = 7, Name = "Iphone 14", Price = 820, CategoryId = 1 },
                new Product { Id = 8, Name = "Iphone 14 Pro", Price = 950, CategoryId = 1 }
            );
        }
    }
}
//SELECT u.UserName, u.Email, r.Name AS RoleName
//FROM AspNetUsers u
//JOIN AspNetUserRoles ur ON u.Id = ur.UserId
//JOIN AspNetRoles r ON ur.RoleId = r.Id
//ORDER BY u.Email
