using PTBlog.Data.Repositories;
using PTBlog.Models;

namespace PTBlog.Endpoints.V1;

public static class PostsEndpointsExtensions
{
    static public WebApplication MapPostsApiEndpoints(this WebApplication app)
    {
        app.MapGet(APIRoutes.Posts.GetAll, async (IPostsRepository repo) =>
        {
            var posts = await repo.GetPostsAsync();
            // TODO: Use another DTO (so we can return the post id etc)
            var postDtos = posts.Select(x => new PostDTO(x.Title, x.Content));
            return Results.Ok(postDtos);
        });

        return app;
    }
}
