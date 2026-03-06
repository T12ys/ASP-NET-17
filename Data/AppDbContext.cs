using ASP_09._Swagger_documentation.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_09._Swagger_documentation.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceRow> InvoiceRows => Set<InvoiceRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(u =>
        {
            u.HasKey(u => u.Id);
            u.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(200);
            u.Property(u => u.Address)
                .HasMaxLength(500);
            u.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);
            u.HasIndex(u => u.Email)
                .IsUnique();
            u.Property(u => u.PasswordHash)
                .IsRequired();
            u.Property(u => u.PhoneNumber)
                .HasMaxLength(50);
            u.Property(u => u.CreatedAt)
                .IsRequired();
            u.Property(u => u.UpdatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(rt =>
        {
            rt.HasKey(rt => rt.Id);
            rt.Property(rt => rt.Token)
                .IsRequired();
            rt.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Customer>(c =>
        {
            c.HasKey(c => c.Id);
            c.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);
            c.Property(c => c.Address)
                .HasMaxLength(500);
            c.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(200);
            c.Property(c => c.PhoneNumber)
                .HasMaxLength(50);
            c.Property(c => c.CreatedAt)
                .IsRequired();
            c.Property(c => c.UpdatedAt)
                .IsRequired();
            c.HasOne(c => c.User)
                .WithMany(u => u.Customers)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Invoice>(i =>
        {
            i.HasKey(i => i.Id);
            i.Property(i => i.StartDate).IsRequired();
            i.Property(i => i.EndDate).IsRequired();
            i.Property(i => i.TotalSum).HasColumnType("decimal(18,2)");
            i.Property(i => i.Comment).HasMaxLength(1000);
            i.Property(i => i.Status).IsRequired();
            i.Property(i => i.CreatedAt).IsRequired();
            i.Property(i => i.UpdatedAt).IsRequired();
            i.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InvoiceRow>(r =>
        {
            r.HasKey(r => r.Id);
            r.Property(r => r.Service).IsRequired().HasMaxLength(300);
            r.Property(r => r.Quantity).HasColumnType("decimal(18,2)");
            r.Property(r => r.Rate).HasColumnType("decimal(18,2)");
            r.Property(r => r.Sum).HasColumnType("decimal(18,2)");
            r.HasOne(r => r.Invoice)
                .WithMany(i => i.Rows)
                .HasForeignKey(r => r.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}