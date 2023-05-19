using Bogus;
using Microsoft.AspNetCore.Identity;
using PTBlog.Claims;
using PTBlog.Models;

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
		await CreateOwnerAsync(userManager);

		const string password = "TestPassword1!";
        var postFaker = CreatePostFaker();
        var userFaker = CreateUserFaker(dbContext, postFaker);
        var users = userFaker.Generate(100);
        foreach (var user in users)
        {
			await userManager.CreateAsync(user, password);
		}
	}

    static private Faker<UserModel> CreateUserFaker(DatabaseContext dbContext, Faker<PostModel> postFaker)
    {
        return new Faker<UserModel>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid().ToString())
            .RuleFor(u => u.ProfilePictureURL, f => f.Internet.Avatar())
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.ApiKey, (_, u) => u.Id)
            .RuleFor(u => u.Posts, (f, u) =>
            {
                postFaker.RuleFor(p => p.AuthorId, _ => u.Id);

                var posts = postFaker.GenerateBetween(1, 5);

                return posts;
            });
    }

	static private Faker<PostModel> CreatePostFaker()
	{
        int _bogusPostId = 1;
        return new Faker<PostModel>()
            .RuleFor(p => p.Id, _ => _bogusPostId++)
            .RuleFor(p => p.Title, f => f.Hacker.Phrase())
            .RuleFor(p => p.Content, f => f.Lorem.Sentences(4))
            .RuleFor(p => p.CreatedDate, f => f.Date.Recent(10))
            .RuleFor(p => p.UpdatedDate, f => f.Date.Recent(10))
            .FinishWith((f, p) =>
            {
                // Make sure updated date is after created date!
                if (p.CreatedDate > p.UpdatedDate)
                {
                    (p.CreatedDate, p.UpdatedDate) = (p.UpdatedDate!.Value, p.CreatedDate);
                }

                // postgreSQL doesn't support offsets in dates so we remove them
                p.CreatedDate = p.CreatedDate.UtcDateTime;
                p.UpdatedDate = p.UpdatedDate!.Value.UtcDateTime;
            });
	}

	static private async Task CreateOwnerAsync(UserManager<UserModel> userManager)
    {
        var userId = Guid.NewGuid().ToString();
        var user = new UserModel()
        {
            Id = userId,
            ProfilePictureURL = "https://www.pngfind.com/pngs/m/676-6764065_default-profile-picture-transparent-hd-png-download.png",
            Username = "Owner",
            ApiKey = userId,	
        };

        const string password = "TestPassword1!";
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, IsAdminRole.Name);
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
