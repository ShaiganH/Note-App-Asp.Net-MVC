using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyNotes.Models;

namespace MyNotes.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>       
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

            
        }
            public DbSet<Note> Notes {  get; set; }
    }
}
