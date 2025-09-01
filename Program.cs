using Microsoft.EntityFrameworkCore;
using VONetData;
using Microsoft.AspNetCore.Identity;
using System.IO;
using VONetData.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/AdminPanel", "Admin");
    options.Conventions.AllowAnonymousToPage("/AccessDenied");
});

// Configure SQLite DbContext and Identity (file in App_Data)
var dataDir = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDir);
var dbPath = Path.Combine(dataDir, "app.db");
var connectionString = $"Data Source={dbPath}";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString)
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// Geocoding service (Nominatim)
builder.Services.AddHttpClient<IGeocodingService, NominatimGeocodingService>(client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("VONetData/1.0 (contact: admin@vonet.org)");
});

var app = builder.Build();

// Optional rebuild (drop) database if configured
var rebuildFlag = builder.Configuration["RebuildDb"] ?? Environment.GetEnvironmentVariable("REBUILD_DB");
if (string.Equals(rebuildFlag, "true", StringComparison.OrdinalIgnoreCase))
{
    if (File.Exists(dbPath))
    {
        File.Delete(dbPath);
    }
}

// Ensure database is created and seed roles/admin user
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    const string adminRole = "Admin";
    const string defaultAdminEmail = "me@jacobkukuk.com";

    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // Promote first user if single user scenario
    var users = userManager.Users.ToList();
    if (users.Count == 1)
    {
        var first = users[0];
        if (!await userManager.IsInRoleAsync(first, adminRole))
        {
            await userManager.AddToRoleAsync(first, adminRole);
        }
    }

    // Ensure specified default admin email (if account exists) has admin role
    var defaultAdminUser = await userManager.FindByEmailAsync(defaultAdminEmail);
    if (defaultAdminUser != null && !await userManager.IsInRoleAsync(defaultAdminUser, adminRole))
    {
        await userManager.AddToRoleAsync(defaultAdminUser, adminRole);
    }
}

// Middleware to auto-promote first registered user to Admin
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Identity/Account/Register") &&
        string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
    {
        // After registration proceeds, run next then check for promotion
        await next.Invoke();
        using var scope = context.RequestServices.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }
        var users = userManager.Users.ToList();
        if (users.Count == 1)
        {
            var first = users[0];
            if (!await userManager.IsInRoleAsync(first, adminRole))
            {
                await userManager.AddToRoleAsync(first, adminRole);
            }
        }
        // Also ensure default admin email retains role
        const string defaultAdminEmail = "me@jacobkukuk.com";
        var defaultAdminUser = await userManager.FindByEmailAsync(defaultAdminEmail);
        if (defaultAdminUser != null && !await userManager.IsInRoleAsync(defaultAdminUser, adminRole))
        {
            await userManager.AddToRoleAsync(defaultAdminUser, adminRole);
        }
        return; // already called next
    }
    await next.Invoke();
});

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

// Force logout if user no longer exists
app.Use(async (context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated ?? false)
    {
        var userManager = context.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
        var signInManager = context.RequestServices.GetRequiredService<SignInManager<IdentityUser>>();
        var userId = userManager.GetUserId(context.User);
        if (!string.IsNullOrEmpty(userId))
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                await signInManager.SignOutAsync();
                context.Response.Redirect("/Login?relogin=1");
                return;
            }
        }
    }
    await next();
});

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
