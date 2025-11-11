using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;
using GoatSilencerArchitecture.Services;
using GoatSilencerArchitecture.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------
// MVC + Razor Pages (Identity UI needs Razor Pages)
// ------------------------------------------------------

// We’ll just register MVC normally; we’ll protect Admin via [Authorize] attributes
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages()
    .AddRazorOptions(options =>
    {
        // So the app can find _LoginPartial under /Identity/Views/Shared
        options.ViewLocationFormats.Add("/Identity/Views/Shared/{0}.cshtml");
    });

// ------------------------------------------------------
// Database (SQLite absolute path)
// ------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbPath = Path.Combine(builder.Environment.ContentRootPath, connectionString!.Replace("Data Source=", ""));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// ------------------------------------------------------
// Custom Services
// ------------------------------------------------------
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<ISettingsService, SettingsService>();
builder.Services.AddScoped<IRichTextValidator, RichTextValidator>();

// ------------------------------------------------------
// Identity (custom ApplicationUser + Roles + Token providers)
// ------------------------------------------------------
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // no 2FA enforced

// ------------------------------------------------------
// Authentication Cookie Configuration
// ------------------------------------------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;

    //  Redirect only /Admin to login; don't affect Public
    options.Events.OnRedirectToLogin = context =>
    {
        var requestPath = context.Request.Path.ToString();

        // Avoid infinite redirect if already on login page
        if (requestPath.StartsWith("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase))
            return Task.CompletedTask;

        // Only redirect to login for Admin area
        if (requestPath.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
        {
            var returnUrl = Uri.EscapeDataString("/Admin");
            context.Response.Redirect($"/Identity/Account/Login?returnUrl={returnUrl}");
        }

        // Otherwise, do nothing (Public area stays accessible)
        return Task.CompletedTask;
    };
});

// ------------------------------------------------------
// Build the app
// ------------------------------------------------------
var app = builder.Build();

// ------------------------------------------------------
// Middleware pipeline
// ------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Public/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (app.Environment.IsDevelopment())
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store");
            ctx.Context.Response.Headers.Append("Pragma", "no-cache");
            ctx.Context.Response.Headers.Append("Expires", "0");
        }
    }
});

app.UseRouting();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------------
// MVC + Razor Page Endpoints
// ------------------------------------------------------
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Public}/{controller=Home}/{action=Index}/{id?}");

// Enables /Identity/... pages (Login, Register, etc.)
app.MapRazorPages();

// ------------------------------------------------------
// Role + Admin seeding
// ------------------------------------------------------
static async Task SeedIdentityDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "Editor", "Writer" };

    // Ensure roles exist
    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Ensure an admin user exists
    var adminEmail = "admin@goatsilencer.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            DisplayName = "Site Administrator",
            RoleDescription = "Full access to CMS"
        };

        //  Change this password after first login
        var result = await userManager.CreateAsync(newAdmin, "Admin@12345");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
            Console.WriteLine("[Seed] Default admin user created successfully.");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"[Seed] Error: {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("[Seed] Admin user already exists.");
    }
}

// Run the seeding logic before starting the app
await SeedIdentityDataAsync(app);

// ------------------------------------------------------
app.Run();
