namespace LMS
{
    public static class WebExtension
    {
            public static void ApplyMigration(this WebApplication app)
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.Migrate();
                }
            }
    }
}
