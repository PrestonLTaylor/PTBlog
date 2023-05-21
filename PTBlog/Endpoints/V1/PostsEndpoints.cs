using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PTBlog.Data.Repositories;
using PTBlog.Endpoints.V1.Requests;
using PTBlog.Endpoints.V1.Responses;
using PTBlog.Models;

namespace PTBlog.Endpoints.V1;

public static class PostsEndpoints
{
	/// <summary>
	/// Gets every post on the supplied page
	/// </summary>
	/// <response code="200">Returns every post on the supplied page</response>
	/// <response code="400">An invalid page number was supplied</response>
	/// <response code="404">A page does not exist for the supplied page number</response>
	static public async Task<IResult> GetAllOnPage(IPostsRepository repo, [FromQuery]int page)
    {
        if (page < 1)
        {
            return Results.BadRequest();
        }

        var posts = await repo.GetPostsOnPageAsync(page);
		if (posts.Count == 0)
		{
			return Results.NotFound();
		}

		var postResponses = posts.Select(x => new PostResponse(x.Id, x.Title, x.Content));
        return Results.Ok(postResponses);
    }

	/// <summary>
	/// Gets a post by its post id
	/// </summary>
	/// <response code="200">Returns a post by its post id</response>
	/// <response code="404">A post does not exist with the supplied id</response>
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

	/// <summary>
	/// Creates a post with the author of the API key
	/// </summary>
	/// <response code="201">The post was created successfully</response>
	/// <response code="400">An empty post title or content was supplied</response>
	/// <response code="401">A valid API key for a user was not supplied</response>
	static public async Task<IResult> Create(IPostsRepository repo, IUsersRepository userRepo, HttpRequest request, [FromBody]CreatePostRequest postRequest)
    {
        if (string.IsNullOrEmpty(postRequest.Title) || string.IsNullOrEmpty(postRequest.Content))
        {
            return Results.BadRequest();
        }

        var author = await userRepo.GetUserFromRequestAsync(request);
        if (author is null)
        {
            return Results.Unauthorized();
        }

        var postDto = new PostDTO(postRequest.Title, postRequest.Content);
        var postId = await repo.AddPostFromDTOAsync(postDto, author);

        var baseUrl = $"{request.Scheme}://{request.Host.ToUriComponent()}";
        var createdUrl = $"{baseUrl}/{APIRoutes.Posts.Get.Replace("{postId}", postId.ToString())}";
        return Results.Created(createdUrl, postId);
    }

	/// <summary>
	/// Edits a post by its post id
	/// </summary>
	/// <response code="204">The post was edited successfully</response>
	/// <response code="400">An empty post title or content was supplied</response>
	/// <response code="401">A valid API key for a user was not supplied</response>
	/// <response code="403">The user does not have access to edit the post</response>
	/// <response code="404">A post does not exist with the supplied id</response>
	static public async Task<IResult> Edit(IPostsRepository repo, IUsersRepository userRepo, HttpRequest request, int postId, [FromBody]EditPostRequest postRequest)
    {
		if (string.IsNullOrEmpty(postRequest.Title) || string.IsNullOrEmpty(postRequest.Content))
		{
			return Results.BadRequest();
		}

		var post = await repo.GetPostByIdAsync(postId);
		if (post is null)
		{
			return Results.NotFound();
		}

        var user = await userRepo.GetUserFromRequestAsync(request);
        if (user is null)
        {
            return Results.Unauthorized();
        }
		if (!await userRepo.DoesUserHaveAccessToPost(user, post))
		{
			return Results.StatusCode(StatusCodes.Status403Forbidden);
		}

        post.Title = postRequest.Title;
        post.Content = postRequest.Content;
        await repo.UpdatePostAsync(post);
        return Results.NoContent();
	}

	/// <summary>
	/// Deletes a post by its post id 
	/// </summary>
	/// <response code="204">The post was deleted successfully</response>
	/// <response code="401">A valid API key for a user was not supplied</response>
	/// <response code="403">The user does not have access to delete the post</response>
	/// <response code="403">A post does not exist with the supplied id</response>
	static public async Task<IResult> Delete(IPostsRepository repo, IUsersRepository userRepo, HttpRequest request, int postId)
    {
        var post = await repo.GetPostByIdAsync(postId);
        if (post is null)
        {
            return Results.NotFound();
        }

		var user = await userRepo.GetUserFromRequestAsync(request);
		if (user is null)
		{
			return Results.Unauthorized();
		}
		if (!await userRepo.DoesUserHaveAccessToPost(user, post))
		{
			return Results.StatusCode(StatusCodes.Status403Forbidden);
		}

		await repo.DeletePostAsync(post);
        return Results.NoContent();
    }
}

public static class PostsEndpointsExtensions
{
    static public WebApplication MapPostsApiEndpoints(this WebApplication app)
    {
        app.MapGet(APIRoutes.Posts.GetAllOnPage, PostsEndpoints.GetAllOnPage);

        app.MapGet(APIRoutes.Posts.Get, PostsEndpoints.Get);

        app.MapPost(APIRoutes.Posts.Create, PostsEndpoints.Create);

        app.MapPut(APIRoutes.Posts.Edit, PostsEndpoints.Edit);

        app.MapDelete(APIRoutes.Posts.Delete, PostsEndpoints.Delete);

        return app;
    }
}
