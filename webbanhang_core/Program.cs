using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using webbanhang_core.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ Kết nối DB
var connectionString = builder.Configuration.GetConnectionString("DBBanHang")
    ?? throw new InvalidOperationException("Connection string 'DBBanHang' not found.");
builder.Services.AddTransient<IEmailSender, FakeEmailSender>();

// ✅ Cấu hình DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ✅ Identity (AppUser + Role)
builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Cho phép login luôn
})
.AddRoles<IdentityRole>() // Thêm roles
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ✅ Razor Pages + MVC
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ✅ Seed dữ liệu Roles + Gán role Admin cho user
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    string[] roles = { "Admin", "Customer", "Employee" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // ✅ Gán role Admin cho user admin2@gmail.com
    var adminUser = await userManager.FindByEmailAsync("admin2@gmail.com");
    if (adminUser != null && !(await userManager.IsInRoleAsync(adminUser, "Admin")))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    var employeeUser = await userManager.FindByEmailAsync("vender3@gmail.com");
    if (employeeUser != null && !(await userManager.IsInRoleAsync(employeeUser, "Employee")))
    {
        await userManager.AddToRoleAsync(employeeUser, "Employee");
    }

}

// ✅ Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Thêm route hỗ trợ Area (QUAN TRỌNG!)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=AdminDashboard}/{action=Index}/{id?}");

// ✅ Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
