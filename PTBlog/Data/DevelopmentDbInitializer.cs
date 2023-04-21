using Microsoft.AspNetCore.Identity;
using PTBlog.Models;

namespace PTBlog.Data;

public static class DevelopmentDbInitializerExtensions
{
    static public async Task<WebApplication> UseDatabaseSeedingAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        dbContext.Database.EnsureCreated();

        if (!dbContext.Users.Any())
        {
            var blogger1Id = CreateDefaultUserWithName(userManager, "Blogger1");
            var blogger2Id = CreateDefaultUserWithName(userManager, "Blogger2");

            CreateDefaultPostWithAuthor(dbContext, blogger1Id);
            CreateDefaultPostWithAuthor(dbContext, blogger2Id);
            await dbContext.SaveChangesAsync();
        }

        return app;
    }

    static private string CreateDefaultUserWithName(UserManager<UserModel> userManager, string username)
    {
        var userId = Guid.NewGuid().ToString();
        var user = new UserModel()
        {
            Id = userId,
            ProfilePictureURL = "https://www.pngfind.com/pngs/m/676-6764065_default-profile-picture-transparent-hd-png-download.png",
            Username = username,
        };

        const string password = "TestPassword1!";
        userManager.CreateAsync(user, password);

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
}
