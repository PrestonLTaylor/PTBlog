using PTBlog.Models;

namespace PTBlog.Data;

public static class DevelopmentDbInitializerExtensions
{
    static public async Task<WebApplication> UseDatabaseSeedingAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        dbContext.Database.EnsureCreated();

        if (!dbContext.Users.Any())
        {
            var blogger1Guid = CreateDefaultUserWithName(dbContext, "Blogger1");
            var blogger2Guid = CreateDefaultUserWithName(dbContext, "Blogger2");

            CreateDefaultPostWithAuthor(dbContext, blogger1Guid);
            CreateDefaultPostWithAuthor(dbContext, blogger2Guid);
            await dbContext.SaveChangesAsync();
        }

        return app;
    }

    static private Guid CreateDefaultUserWithName(DatabaseContext context, string username)
    {
        var userGuid = Guid.NewGuid();
        context.Users.Add(new UserModel()
        {
            Id = userGuid,
            ProfilePictureURL = "https://www.pngfind.com/pngs/m/676-6764065_default-profile-picture-transparent-hd-png-download.png",
            Username = username,
        });

        return userGuid;
    }

    static private void CreateDefaultPostWithAuthor(DatabaseContext context, Guid authorGuid)
    {
        context.Posts.Add(new PostModel()
        {
            AuthorGuid = authorGuid,
            Title = "Default Post",
            Content = $"This is a development blog for Author {authorGuid}!",
            CreatedDate = DateTimeOffset.UtcNow.AddDays(-1),
            UpdatedDate = DateTimeOffset.UtcNow,
        });
    }
}
