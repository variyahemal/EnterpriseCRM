using Microsoft.EntityFrameworkCore;
using EnterpriseCRM.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EnterpriseCRM.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<SaleOpportunity> SaleOpportunities { get; set; }
        public DbSet<FollowUp> FollowUps { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<ContentPage> ContentPages { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<BlogPostCategory> BlogPostCategories { get; set; }
        public DbSet<BlogPostTag> BlogPostTags { get; set; }
        public DbSet<AppUserRole> AppUserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUserRole>(b =>
            {
                b.HasKey(ur => new { ur.UserId, ur.RoleId });

                b.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                b.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<BlogPostCategory>(b =>
            {
                b.HasKey(bc => new { bc.BlogPostId, bc.CategoryId });

                b.HasOne(bc => bc.BlogPost)
                    .WithMany(b => b.BlogPostCategories)
                    .HasForeignKey(bc => bc.BlogPostId)
                    .IsRequired();

                b.HasOne(bc => bc.Category)
                    .WithMany(c => c.BlogPostCategories)
                    .HasForeignKey(bc => bc.CategoryId)
                    .IsRequired();
            });

            builder.Entity<BlogPostTag>(b =>
            {
                b.HasKey(bt => new { bt.BlogPostId, bt.TagId });

                b.HasOne(bt => bt.BlogPost)
                    .WithMany(b => b.BlogPostTags)
                    .HasForeignKey(bt => bt.BlogPostId)
                    .IsRequired();

                b.HasOne(bt => bt.Tag)
                    .WithMany(t => t.BlogPostTags)
                    .HasForeignKey(bt => bt.TagId)
                    .IsRequired();
            });

            // Configure relationships for other entities
            builder.Entity<Contact>()
                .HasOne(c => c.Lead)
                .WithMany(l => l.Contacts)
                .HasForeignKey(c => c.LeadId)
                .IsRequired(false);

            builder.Entity<Contact>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.Contacts)
                .HasForeignKey(c => c.OwnerId)
                .IsRequired(false);

            builder.Entity<Lead>()
                .HasOne(l => l.AssignedUser)
                .WithMany(u => u.AssignedLeads)
                .HasForeignKey(l => l.AssignedUserId)
                .IsRequired(false);

            builder.Entity<SaleOpportunity>()
                .HasOne(s => s.Contact)
                .WithMany(c => c.SaleOpportunities)
                .HasForeignKey(s => s.ContactId)
                .IsRequired();

            builder.Entity<FollowUp>()
                .HasOne(f => f.Contact)
                .WithMany(c => c.FollowUps)
                .HasForeignKey(f => f.ContactId)
                .IsRequired();

            builder.Entity<BlogPost>()
                .HasOne(b => b.Author)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(b => b.AuthorId)
                .IsRequired();

            builder.Entity<MediaFile>()
                .HasOne(m => m.Uploader)
                .WithMany(u => u.MediaFiles)
                .HasForeignKey(m => m.UploadedBy)
                .IsRequired();

            builder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .IsRequired();

            builder.Entity<SaleOpportunity>()
                .Property(s => s.Value)
                .HasPrecision(18, 2);
        }
    }
}