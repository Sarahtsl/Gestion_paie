using Gestion_paie.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Gestion_paie.DataBase
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Companies { get; set; }

        public DbSet<CnssRate> CnssRates { get; set; }
        public DbSet<TaxBracket> TaxBrackets { get; set; }
        public DbSet<TaxDeduction> TaxDeductions { get; set; }

        public DbSet<PayrollPeriod> PayrollPeriods { get; set; }
        
        public DbSet<RuleName> RuleNames { get; set; }
        public DbSet<AnomalyRule> AnomalyRules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeNumber)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.CNSSNumber)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.CIN)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.UserId)
                .IsUnique();
            modelBuilder.Entity<PayrollPeriod>()
                   .Property(p => p.CreatedAt)
                 .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<BenefitType>()
            .HasIndex(bt => bt.Name)
            .IsUnique();

          


            // Unicité du nom de règle
            modelBuilder.Entity<RuleName>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // FK AnomalyRule → RuleName (pas de cascade sur un référentiel)
            modelBuilder.Entity<AnomalyRule>()
                .HasOne(a => a.RuleName)
                .WithMany()
                .HasForeignKey(a => a.RuleNameId)
                .OnDelete(DeleteBehavior.Restrict);

            // Stocker les enums en VARCHAR (au lieu d'int)
            modelBuilder.Entity<AnomalyRule>()
                .Property(a => a.RuleType)
                .HasConversion<string>()
                .HasMaxLength(50);

            modelBuilder.Entity<AnomalyRule>()
                .Property(a => a.Severity)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Valeur par défaut SQL Server
            modelBuilder.Entity<AnomalyRule>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

        }
    }
}
