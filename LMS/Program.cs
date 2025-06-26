using LMS.Data;
using LMS;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using LMS.Models;
using LMS.Repositories;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using LMS.Services;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure()
    )
); 
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy<string>("default", HttpContext =>
        {
            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    Window = TimeSpan.FromSeconds(10),
                    PermitLimit = 10,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
        });
});
builder.Services.AddTransient<IHomeRepository, HomeRepository>(); 
builder.Services.AddTransient<IModuleRepository, ModuleRepository>(); 
builder.Services.AddTransient<IEnrollRepository, EnrollRepository>();
builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
builder.Services.AddTransient<IEmailService,EmailService>();
builder.Services.AddTransient<INotificationService, NotiService>();

//expire time for email tokens
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromHours(2); 
});

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

app.ApplyMigration();

app.UseRouting();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    Env.Load();
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new string[] { "Admin", "Teacher", "Student" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL"); 
    string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASS");
    string adminUsername = Environment.GetEnvironmentVariable("ADMIN_USER");

    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new IdentityUser { UserName = adminUsername, Email = adminEmail };
        admin.EmailConfirmed = true;
        await userManager.CreateAsync(admin, adminPassword);
        await userManager.AddToRoleAsync(admin, "Admin");
    }

}


app.MapHub<NotiHub>("/notihub");
app.Run();