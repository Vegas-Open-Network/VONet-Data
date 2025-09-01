using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VONetData.Models;

namespace VONetData
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Member> Members { get; set; }
        public DbSet<FeatureRequest> FeatureRequests { get; set; }
        public DbSet<MemberNote> MemberNotes { get; set; }
        public DbSet<InteractionLog> InteractionLogs { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<NetworkLink> NetworkLinks { get; set; }
    }
}
