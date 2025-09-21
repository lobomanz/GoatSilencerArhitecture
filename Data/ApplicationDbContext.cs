using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GoatSilencerArchitecture.Models;

namespace GoatSilencerArchitecture.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<GalleryImage> GalleryImages { get; set; }
        public DbSet<AboutComponent> AboutComponents { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
    }
}
