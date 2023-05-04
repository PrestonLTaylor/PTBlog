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

        app.MapGet(APIRoutes.Posts.Get, async (IPostsRepository repo, int postId) =>
        {
            var post = await repo.GetPostByIdAsync(postId);
            if (post is null)
            {
                return Results.NotFound();
            }

            var postDto = new PostDTO(post.Title, post.Content);
            return Results.Ok(postDto);
        });

        return app;
    }
}
