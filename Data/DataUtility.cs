using ContactPro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ContactPro.Data
{
    public static class DataUtility
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection"); // Project is in Development
            string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL"); //Project is published
            return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl);
        }
        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder()
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {

            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<AppUser>>();
            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();

            //Align the database by checking Migrations
            await dbContextSvc.Database.MigrateAsync();

            //Seed Demo User(s)
            await SeedDemoUsersAsync(userManagerSvc, configurationSvc);
        }

        // Demo Users Seed Method
        private static async Task SeedDemoUsersAsync(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            string? demoLoginEmail = configuration["DemoLoginEmail"] ?? Environment.GetEnvironmentVariable("DemoLoginEmail");
            string? demoPassword = configuration["DemoLoginPassword"] ?? Environment.GetEnvironmentVariable("DemoLoginPassword");

            AppUser demoUser = new AppUser()
            {
                UserName = demoLoginEmail,
                Email = demoLoginEmail,
                FirstName = "Demo",
                LastName = "User",
                EmailConfirmed = true
            };
            try
            {
                AppUser? appUser = await userManager.FindByEmailAsync(demoLoginEmail!);

                if (appUser == null)
                {
                    await userManager.CreateAsync(demoUser, demoPassword!);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("********** ERROR **********");
                Console.WriteLine("Error Seeding Demo Login User");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***************************");




                throw;
            }

        }
    }
}
