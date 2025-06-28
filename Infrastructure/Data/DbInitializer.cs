using Microsoft.EntityFrameworkCore;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Core.Helper;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Data;
public static class DbInitializer
{
    public static async Task SeedData(ApplicationContext context)
    {
        await SeedRoles(context);
        await SeedSuperAdmin(context);
    }

    private static async Task SeedRoles(ApplicationContext context)
    {
        if (await context.Roles.AnyAsync())
            return;

        var roles = new List<Role>
        {
            new() {
                Id = Guid.NewGuid(),
                Name = "SuperAdmin",
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Admin"
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Teacher"
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Student"
            }
        };

        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSuperAdmin(ApplicationContext context)
    {
        var superAdminExists = await context.UserRoles
            .AnyAsync(a => a.Role != null && a.Role.Name == "SuperAdmin" && !a.IsDeleted);

        if (superAdminExists) return;

        var superAdminRole = await context.Roles
            .FirstOrDefaultAsync(r => r.Name == "SuperAdmin");

        if (superAdminRole == null)
        {
            throw new InvalidOperationException("SuperAdmin role not found. Ensure roles are seeded first.");
        }

        var defaultPassword = "SuperAdmin123!";
        string saltString = HashingHelper.GenerateSalt();
        string hashedPassword = HashingHelper.HashPassword(defaultPassword, saltString);

        var superAdminUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "SuperAdmin",
            Email = "superadmin@school.com",
            HashSalt = saltString,
            PasswordHash = hashedPassword,
            CreatedOn = DateTime.UtcNow
        };

        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = superAdminUser.Id,
            RoleId = superAdminRole.Id,
            CreatedOn = DateTime.UtcNow
        };

        var superAdmin = new Admin
        {
            Id = Guid.NewGuid(),
            FirstName = "Super",
            LastName = "Admin",
            Email = "superadmin@school.com",
            UserId = superAdminUser.Id,
            User = superAdminUser,
            DateOfBirth = new DateOnly(1990, 1, 1),
            PhoneNumber = "1234567890",
            Gender = "Male",
            Address = "System Generated",
            CreatedOn = DateTime.UtcNow,
            IsDeleted = false
        };

        context.Users.Add(superAdminUser);
        context.UserRoles.Add(userRole);
        context.Admins.Add(superAdmin);

        await context.SaveChangesAsync();
    }
}