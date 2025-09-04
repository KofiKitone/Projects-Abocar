global using Abocar.Data;
global using Abocar.Models;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity.UI.Services;
using System.Drawing.Text;
using SQLitePCL;

//dependency injection
var builder = WebApplication.CreateBuilder(args);
// services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<ICartWishlistService, CartWishlistService>();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the timeout duration for session
    options.Cookie.HttpOnly = true; // Make the session cookie HttpOnly
    options.Cookie.IsEssential = true; // Mark the session cookie as essential
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultUI()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.AddScoped<CategoryDropdownViewComponent>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Warning);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure this is before UseAuthorization
app.UseAuthorization();

app.UseSession();

// Role and User seeding
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Seed roles
    var roles = new[] { "Administrator", "Vendor", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed users
    var users = new List<(string FirstName, string LastName, string Email, string Role)>
    {
        ("John", "Doe", "john.doe@example.com", "Administrator"),
        ("Mary", "Smith", "mary.smith@example.com", "Vendor"),
        ("Kwame", "Mensah", "kwame.mensah@example.com", "User"),
        ("Fatima", "Hassan", "fatima.hassan@example.com", "User"),
        ("Carlos", "Lopez", "carlos.lopez@example.com", "User"),
        ("Akosua", "Boateng", "akosua.boateng@example.com", "User"),
        ("James", "Brown", "james.brown@example.com", "User"),
        ("Sophia", "Williams", "sophia.williams@example.com", "User"),
        ("Chen", "Wei", "chen.wei@example.com", "User"),
        ("Aisha", "Khan", "aisha.khan@example.com", "User"),
        ("David", "Johnson", "david.johnson@example.com", "User"),
        ("Ama", "Owusu", "ama.owusu@example.com", "User"),
        ("Omar", "Ali", "omar.ali@example.com", "User"),
        ("Elena", "Garcia", "elena.garcia@example.com", "User"),
        ("Michael", "Anderson", "michael.anderson@example.com", "User"),
        ("Grace", "Adjei", "grace.adjei@example.com", "User"),
        ("Samuel", "Ofori", "samuel.ofori@example.com", "User"),
        ("Isabella", "Martinez", "isabella.martinez@example.com", "User"),
        ("Daniel", "Park", "daniel.park@example.com", "User"),
        ("Linda", "Nguyen", "linda.nguyen@example.com", "User"),
    };

    foreach (var (firstName, lastName, email, role) in users)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                // if ApplicationUser has extra props like FirstName/LastName, set them
                FirstName = firstName,
                LastName = lastName,
                Nationaltiy="Ghanaian"
            };

            var result = await userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
            else
            {
                Console.WriteLine($"Failed to create {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
