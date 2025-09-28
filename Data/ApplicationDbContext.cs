using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GoatSilencerArchitecture.Models;

namespace GoatSilencerArchitecture.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ImageModel> ProjectImages { get; set; }
        public DbSet<BlogComponent> BlogComponents { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
    }
}
