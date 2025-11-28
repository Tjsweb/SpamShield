using InstaHyreSDETest.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace InstaHyreSDETest.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
             : base(options)
        {
        }

        public DbSet<Contacts> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Contacts>()
                .HasOne<AppUser>(s => s.User)
                .WithMany(g => g.Contacts)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Contacts>()
                .HasOne<AppUser>(s => s.ContactUser)
                .WithMany()
                .HasForeignKey(s => s.ContactUserId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Contacts>()
                .Ignore(s => s.User);
        }
    }
}
