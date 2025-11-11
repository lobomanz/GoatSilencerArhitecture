using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GoatSilencerArchitecture.Models;

namespace GoatSilencerArchitecture.Data
{
    // NOTE: changed from IdentityDbContext to IdentityDbContext<ApplicationUser>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ImageModel> BlogImages { get; set; }
        public DbSet<BlogComponent> Blogs { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<AboutUsSection> AboutUsSections { get; set; }
    }
}
