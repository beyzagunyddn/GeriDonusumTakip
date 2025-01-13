using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GeriDonusumTakip.Models;

namespace GeriDonusumTakip.Data
{
    public class UygulamaDbContext : IdentityDbContext<UygulamaKullanici>
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options)
            : base(options)
        {
        }

        public DbSet<GeriDonusum> GeriDonusumler { get; set; }
        public DbSet<KisiselHedef> KisiselHedefler { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<KisiselHedef>()
                .HasOne<UygulamaKullanici>()
                .WithMany()
                .HasForeignKey(h => h.KullaniciId);
        }
    }
} 