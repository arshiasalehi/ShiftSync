using Microsoft.EntityFrameworkCore;
using ShiftSync.Api.Infrastructure;
using ShiftSync.Api.Models;
using ShiftSync.Api.Utilities;
using ShiftSync.Shared.Contracts;

namespace ShiftSync.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ShiftSyncDbContext dbContext, IPasswordHasher passwordHasher)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (!await dbContext.RoleTypes.AnyAsync())
        {
            dbContext.RoleTypes.AddRange(
                new RoleType { Name = "Kitchen" },
                new RoleType { Name = "Cash" },
                new RoleType { Name = "Drinks" });

            await dbContext.SaveChangesAsync();
        }

        if (await dbContext.Businesses.AnyAsync())
        {
            return;
        }

        var roles = await dbContext.RoleTypes.OrderBy(x => x.Name).ToListAsync();
        var kitchen = roles.First(x => x.Name == "Kitchen");
        var cash = roles.First(x => x.Name == "Cash");
        var drinks = roles.First(x => x.Name == "Drinks");

        var business = new Business
        {
            Name = "Sunset Diner",
            CompanyCode = "SUN123"
        };

        var admin = new User
        {
            FullName = "Sam Admin",
            Email = "admin@sunset.local",
            PasswordHash = passwordHasher.Hash("Admin123!"),
            Role = UserRoles.Admin,
            Business = business
        };

        var employee1 = new User
        {
            FullName = "Alice Cook",
            Email = "alice@sunset.local",
            PasswordHash = passwordHasher.Hash("Employee123!"),
            Role = UserRoles.Employee,
            Business = business,
            EmployeeProfile = new EmployeeProfile
            {
                HourlyRate = 22,
                MaxWeeklyHours = 40
            }
        };

        var employee2 = new User
        {
            FullName = "Ben Bar",
            Email = "ben@sunset.local",
            PasswordHash = passwordHasher.Hash("Employee123!"),
            Role = UserRoles.Employee,
            Business = business,
            EmployeeProfile = new EmployeeProfile
            {
                HourlyRate = 19,
                MaxWeeklyHours = 32
            }
        };

        business.Users.Add(admin);
        business.Users.Add(employee1);
        business.Users.Add(employee2);

        dbContext.Businesses.Add(business);
        await dbContext.SaveChangesAsync();

        dbContext.EmployeeRoles.AddRange(
            new EmployeeRole { EmployeeId = employee1.Id, RoleTypeId = kitchen.Id },
            new EmployeeRole { EmployeeId = employee1.Id, RoleTypeId = cash.Id },
            new EmployeeRole { EmployeeId = employee2.Id, RoleTypeId = drinks.Id });

        dbContext.Availabilities.AddRange(
            new Availability { EmployeeId = employee1.Id, DayOfWeek = 1, StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(13) },
            new Availability { EmployeeId = employee1.Id, DayOfWeek = 1, StartTime = TimeSpan.FromHours(14), EndTime = TimeSpan.FromHours(20) },
            new Availability { EmployeeId = employee1.Id, DayOfWeek = 3, StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(18) },
            new Availability { EmployeeId = employee2.Id, DayOfWeek = 2, StartTime = TimeSpan.FromHours(12), EndTime = TimeSpan.FromHours(22) },
            new Availability { EmployeeId = employee2.Id, DayOfWeek = 5, StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(18) });

        var weekStart = WeekDateHelper.NormalizeWeekStart(DateTime.UtcNow.Date);

        var schedule = new WeeklySchedule
        {
            BusinessId = business.Id,
            WeekStartDate = weekStart,
            IsPublished = true
        };

        dbContext.WeeklySchedules.Add(schedule);
        await dbContext.SaveChangesAsync();

        dbContext.Shifts.AddRange(
            new Shift
            {
                WeeklyScheduleId = schedule.Id,
                EmployeeId = employee1.Id,
                RoleTypeId = kitchen.Id,
                ShiftDate = weekStart.AddDays(1),
                StartTime = TimeSpan.FromHours(14),
                EndTime = TimeSpan.FromHours(18)
            },
            new Shift
            {
                WeeklyScheduleId = schedule.Id,
                EmployeeId = employee2.Id,
                RoleTypeId = drinks.Id,
                ShiftDate = weekStart.AddDays(2),
                StartTime = TimeSpan.FromHours(12),
                EndTime = TimeSpan.FromHours(18)
            });

        await dbContext.SaveChangesAsync();
    }
}
