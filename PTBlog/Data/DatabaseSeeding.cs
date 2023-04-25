using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PTBlog.Claims;
using PTBlog.Models;
using System.Security.Claims;
using static System.Formats.Asn1.AsnWriter;

namespace PTBlog.Data;

public static class DevelopmentDbInitializerExtensions
{
    static public async Task<WebApplication> UseDatabaseSeedingAsync(this WebApplication app, bool isDevelopment)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        if (isDevelopment)
        {
            await ResetDatabaseAndSeedDefaultUsersAsync(scope, dbContext);
        }
        else
        {
            await AddAdminRoleAsync(scope, dbContext);
        }

        await dbContext.SaveChangesAsync();
		return app;
    }

    static private async Task ResetDatabaseAndSeedDefaultUsersAsync(IServiceScope scope, DatabaseContext dbContext)
    {
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();

		await dbContext.Database.EnsureDeletedAsync();
		await dbContext.Database.EnsureCreatedAsync();

		await AddAdminRoleAsync(scope, dbContext);

		if (!dbContext.Users.Any())
		{
			await CreateDefaultUserWithNameAsync(userManager, "Owner", isAdmin: true);
			var blogger1Id = await CreateDefaultUserWithNameAsync(userManager, "Blogger1", isAdmin: false);
			var blogger2Id = await CreateDefaultUserWithNameAsync(userManager, "Blogger2", isAdmin: false);

			CreateDefaultPostWithAuthor(dbContext, blogger1Id);
			CreateDefaultPostWithAuthor(dbContext, blogger2Id);
		}
	}

    static private async Task<string> CreateDefaultUserWithNameAsync(UserManager<UserModel> userManager, string username, bool isAdmin)
    {
        var userId = Guid.NewGuid().ToString();
        var user = new UserModel()
        {
            Id = userId,
            ProfilePictureURL = "https://www.pngfind.com/pngs/m/676-6764065_default-profile-picture-transparent-hd-png-download.png",
            Username = username,
        };

        const string password = "TestPassword1!";
        await userManager.CreateAsync(user, password);

        if (isAdmin)
        {
            await userManager.AddToRoleAsync(user, IsAdminRole.Name);
        }

        return userId;
    }

    static private void CreateDefaultPostWithAuthor(DatabaseContext context, string authorId)
    {
        context.Posts.Add(new PostModel()
        {
            AuthorId = authorId,
            Title = "Default Post",
            Content = $"This is a development blog for Author {authorId}!",
            CreatedDate = DateTimeOffset.UtcNow.AddDays(-1),
            UpdatedDate = DateTimeOffset.UtcNow,
        });
    }

    static private async Task AddAdminRoleAsync(IServiceScope scope, DatabaseContext context)
	{
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		if (!await roleManager.RoleExistsAsync(IsAdminRole.Name))
		{
			await roleManager.CreateAsync(new IdentityRole(IsAdminRole.Name));
		}
	}
}
