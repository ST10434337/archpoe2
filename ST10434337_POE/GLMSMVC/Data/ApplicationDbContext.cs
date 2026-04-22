using GLMSMVC.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace GLMSMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)//(Teddy Smith, 2022)
        {           
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)//(Teddy Smith, 2022)
        {
            base.OnModelCreating(modelBuilder);

            // Client -> Contracts (One-to-Many)
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Contracts)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Contract -> ServiceRequest (One-to-One)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.ServiceRequest)
                .WithOne(sr => sr.Contract)
                .HasForeignKey<ServiceRequest>(sr => sr.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Client constraints
            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ContactDetails)
                    .HasMaxLength(100);

                entity.Property(e => e.Region)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // Contract constraints
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.Property(e => e.FilePath)
                    .HasMaxLength(255);

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.ServiceLevel)
                    .HasConversion<string>();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion();
            });

            // ServiceRequest constraints
            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.SourceCurrency)
                    .HasMaxLength(10);

                entity.Property(e => e.CostZar)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.OriginalAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(e => e.UsedAPI)
                    .IsRequired()
                    .HasDefaultValue(false);
            });
        }

    }
}
/*
 Teddy Smith. 2022. ASP.NET Core MVC 2022 - 3. Installing Entity Framework + DB Context [video online]. Avaliable at:<https://youtu.be/af_tK9LUiX0?si=QfKJlebPFrKioaM8> [Accessed 22 April 2026].
 */
