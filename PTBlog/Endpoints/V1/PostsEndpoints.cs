﻿using Microsoft.AspNetCore.Mvc;
using PTBlog.Data.Repositories;
using PTBlog.Endpoints.V1.Requests;
using PTBlog.Models;

namespace PTBlog.Endpoints.V1;

public static class PostsEndpoints
{
    static public async Task<IResult> GetAll(IPostsRepository repo)
    {
        var posts = await repo.GetPostsAsync();
        // TODO: Use another DTO (so we can return the post id etc)
        var postDtos = posts.Select(x => new PostDTO(x.Title, x.Content));
        return Results.Ok(postDtos);
    }

    static public async Task<IResult> Get(IPostsRepository repo, int postId)
    {
        var post = await repo.GetPostByIdAsync(postId);
        if (post is null)
        {
            return Results.NotFound();
        }

        var postDto = new PostDTO(post.Title, post.Content);
        return Results.Ok(postDto);
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
}

public static class PostsEndpointsExtensions
{
    static public WebApplication MapPostsApiEndpoints(this WebApplication app)
    {
        app.MapGet(APIRoutes.Posts.GetAll, PostsEndpoints.GetAll);

        app.MapGet(APIRoutes.Posts.Get, PostsEndpoints.Get);

        app.MapPost(APIRoutes.Posts.Create, PostsEndpoints.Create);

        return app;
    }
}
