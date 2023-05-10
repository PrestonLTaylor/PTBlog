using Microsoft.AspNetCore.Mvc;
using PTBlog.Data.Repositories;
using PTBlog.Endpoints.V1.Requests;
using PTBlog.Endpoints.V1.Responses;
using PTBlog.Models;

namespace PTBlog.Endpoints.V1;

public static class PostsEndpoints
{
    static public async Task<IResult> GetAll(IPostsRepository repo)
    {
        var posts = await repo.GetPostsAsync();
        var postResponses = posts.Select(x => new PostResponse(x.Id, x.Title, x.Content));
        return Results.Ok(postResponses);
    }

    static public async Task<IResult> Get(IPostsRepository repo, int postId)
    {
        var post = await repo.GetPostByIdAsync(postId);
        if (post is null)
        {
            return Results.NotFound();
        }

        var postResponse = new PostResponse(post.Id, post.Title, post.Content);
        return Results.Ok(postResponse);
    }

    static public async Task<IResult> Create(IPostsRepository repo, IUsersRepository userRepo, HttpContext context, [FromBody]CreatePostRequest postRequest)
    {
        // TODO: GetUserByAPIKey instead of from using the claim
        var author = await userRepo.GetUserByClaimAsync(context.User);
        if (author is null)
        {
            return Results.Unauthorized();
        }

        var postDto = new PostDTO(postRequest.Title, postRequest.Content);
        var postId = await repo.AddPostFromDTOAsync(postDto, author);

        var baseUrl = $"{context.Request.Scheme}://{context.Request.Host.ToUriComponent()}";
        var createdUrl = $"{baseUrl}/{APIRoutes.Posts.Get.Replace("{postId}", postId.ToString())}";
        return Results.Created(createdUrl, postId);
    }

    static public async Task<IResult> Edit(IPostsRepository repo, IUsersRepository userRepo, HttpContext context, int postId, [FromBody]EditPostRequest postRequest)
    {
		var post = await repo.GetPostByIdAsync(postId);
		if (post is null)
		{
			return Results.NotFound();
		}

        var user = await userRepo.GetUserByClaimAsync(context.User);
        if (user is null)
        {
            return Results.Unauthorized();
        }

		if (!await userRepo.DoesUserHaveAccessToPost(user, post))
		{
			return Results.Forbid();
		}

        post.Title = postRequest.Title;
        post.Content = postRequest.Content;
        await repo.UpdatePostAsync(post);
        return Results.NoContent();
	}

    static public async Task<IResult> Delete(IPostsRepository repo, IUsersRepository userRepo, HttpContext context, int postId)
    {
        var post = await repo.GetPostByIdAsync(postId);
        if (post is null)
        {
            return Results.NotFound();
        }

		var user = await userRepo.GetUserByClaimAsync(context.User);
		if (user is null)
		{
			return Results.Unauthorized();
		}

		if (!await userRepo.DoesUserHaveAccessToPost(user, post))
		{
			return Results.Forbid();
		}

		await repo.DeletePostAsync(post);
        return Results.NoContent();
    }
}

public static class PostsEndpointsExtensions
{
    static public WebApplication MapPostsApiEndpoints(this WebApplication app)
    {
        app.MapGet(APIRoutes.Posts.GetAll, PostsEndpoints.GetAll);

        app.MapGet(APIRoutes.Posts.Get, PostsEndpoints.Get);

        app.MapPost(APIRoutes.Posts.Create, PostsEndpoints.Create);

        app.MapPut(APIRoutes.Posts.Edit, PostsEndpoints.Edit);

        app.MapDelete(APIRoutes.Posts.Delete, PostsEndpoints.Delete);

        return app;
    }
}
