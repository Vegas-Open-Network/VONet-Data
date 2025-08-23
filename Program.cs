using Microsoft.EntityFrameworkCore;
using VONetData;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/AdminPanel", "Admin");
    options.Conventions.AllowAnonymousToPage("/AccessDenied");
});

// Add MySQL DbContext and Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0))
    )
);
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Automatically apply migrations and create tables at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    // Ensure Admin role exists and assign to me@jacobkukuk.com
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    const string adminEmail = "me@jacobkukuk.com";
    const string adminRole = "Admin";
    if (!roleManager.RoleExistsAsync(adminRole).Result)
    {
        roleManager.CreateAsync(new IdentityRole(adminRole)).Wait();
    }
    var adminUser = userManager.FindByEmailAsync(adminEmail).Result;
    if (adminUser != null && !userManager.IsInRoleAsync(adminUser, adminRole).Result)
    {
        userManager.AddToRoleAsync(adminUser, adminRole).Wait();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Custom access denied handling
app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 403)
    {
        context.HttpContext.Response.Redirect("/Account/AccessDenied");
    }
});

app.Run();
