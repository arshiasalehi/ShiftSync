using Microsoft.EntityFrameworkCore;
using ShiftSync.Api.Models;

namespace ShiftSync.Api.Data;

public sealed class ShiftSyncDbContext(DbContextOptions<ShiftSyncDbContext> options) : DbContext(options)
{
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<User> Users => Set<User>();
    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();
    public DbSet<RoleType> RoleTypes => Set<RoleType>();
    public DbSet<EmployeeRole> EmployeeRoles => Set<EmployeeRole>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<WeeklySchedule> WeeklySchedules => Set<WeeklySchedule>();
    public DbSet<Shift> Shifts => Set<Shift>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Business>()
            .HasIndex(x => x.CompanyCode)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(x => x.Business)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeProfile>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        modelBuilder.Entity<EmployeeProfile>()
            .Property(x => x.HourlyRate)
            .HasPrecision(18, 2);

        modelBuilder.Entity<EmployeeProfile>()
            .HasOne(x => x.User)
            .WithOne(x => x.EmployeeProfile)
            .HasForeignKey<EmployeeProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmployeeRole>()
            .HasIndex(x => new { x.EmployeeId, x.RoleTypeId })
            .IsUnique();

        modelBuilder.Entity<EmployeeRole>()
            .HasOne(x => x.Employee)
            .WithMany(x => x.EmployeeRoles)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmployeeRole>()
            .HasOne(x => x.RoleType)
            .WithMany(x => x.EmployeeRoles)
            .HasForeignKey(x => x.RoleTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Availability>()
            .HasOne(x => x.Employee)
            .WithMany(x => x.Availabilities)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WeeklySchedule>()
            .HasIndex(x => new { x.BusinessId, x.WeekStartDate })
            .IsUnique();

        modelBuilder.Entity<WeeklySchedule>()
            .HasOne(x => x.Business)
            .WithMany(x => x.WeeklySchedules)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Shift>()
            .HasOne(x => x.WeeklySchedule)
            .WithMany(x => x.Shifts)
            .HasForeignKey(x => x.WeeklyScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Shift>()
            .HasOne(x => x.Employee)
            .WithMany(x => x.Shifts)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Shift>()
            .HasOne(x => x.RoleType)
            .WithMany(x => x.Shifts)
            .HasForeignKey(x => x.RoleTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
