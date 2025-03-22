using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using TodoApp.DataAccess;
using TodoApp.Reposotory;
using TodoApp.Reposotory.IRepository;
using Microsoft.AspNetCore.Identity;
using TodoApp.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using TodoApp.Utilties.EmailSender;

namespace TodoApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(option => {
                option.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthentication()
  .AddGoogle(GoogleOptions =>
  {
      GoogleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
      GoogleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
  });
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/ErrorPage/AccessDenied/Errormodel";      
            });

            builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
            QuestPDF.Settings.License = LicenseType.Community;


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=User}/{controller=Home}/{action=Index}/{id?}");

            app.Run();

        }
    }
}
