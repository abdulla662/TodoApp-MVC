using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;
using TodoApp.Models.ViewModel;

namespace TodoApp.DataAccess
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public DbSet<TaskItem> Items { get; set; }
        public DbSet<EditUserViewModel> EditUsers { get; set; }

        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
 : base(options)
        {
        }
        public ApplicationDbContext()
        {
            
        }
        //legacy code 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=TodoApp;Integrated Security=True;TrustServerCertificate=True");
        }
    }
}
