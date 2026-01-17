using HindiBookStore.Data;
using HindiBookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DATABASE CONFIGURATION - Environment Based
if (builder.Environment.IsDevelopment())
{
      // Local Development: SQLite
      var sqliteConnection = builder.Configuration.GetConnectionString("SqliteConnection");
      builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                                                  options.UseSqlite(sqliteConnection));
}
else
{
      // Production: Check for Fly.io PostgreSQL or Somee SQL Server
      var flyDbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

      if (!string.IsNullOrEmpty(flyDbUrl))
      {
                // Fly.io: PostgreSQL (DATABASE_URL is auto-set by Fly.io)
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                                                                options.UseNpgsql(flyDbUrl));
      }
      else
      {
                // Somee: SQL Server (fallback)
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                                                                options.UseSqlServer(connectionString));
      }
}

// IDENTITY
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();

var app = builder.Build();

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

app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
