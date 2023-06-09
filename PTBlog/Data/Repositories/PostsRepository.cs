﻿using Microsoft.EntityFrameworkCore;
using PTBlog.Models;

namespace PTBlog.Data.Repositories;

public sealed class PostsRepository : IPostsRepository
{
	public PostsRepository(DatabaseContext dbContext)
	{
		_dbContext = dbContext ?? throw new InvalidOperationException($"A null database context was provided to {nameof(PostsRepository)}");
	}

	public async Task<List<PostModel>> GetPostsAsync()
	{
		return await GetPostsOrderedByCreationDate().ToListAsync();
	}

	public async Task<List<PostModel>> GetPostsOnPageAsync(int pageNumber)
	{
		var postsToSkip = (pageNumber - 1) * POSTS_PER_PAGE;
		return await GetPostsOrderedByCreationDate().Skip(postsToSkip).Take(POSTS_PER_PAGE).ToListAsync();
	}

	public async Task<PostModel?> GetLatestPostAsync()
	{
		return await GetPostsOrderedByCreationDate().FirstOrDefaultAsync();
	}

	public async Task<List<PostModel>> GetPostsByTitleAsync(string wantedTitle)
	{
		return await GetPostsOrderedByCreationDate().Where(x => x.Title.Contains(wantedTitle)).ToListAsync();
	}

	public async Task<PostModel?> GetPostByIdAsync(int? postId)
	{
		if (postId is null)
		{
			return null;
		}

		return await GetPostsWithTheirRelations().FirstOrDefaultAsync(p => p.Id == postId);
	}

	public async Task AddPostAsync(PostModel model)
	{
		await _dbContext.Posts.AddAsync(model);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<int> AddPostFromDTOAsync(PostDTO postDto, UserModel author)
	{
        var model = new PostModel { Title = postDto.Title, Content = postDto.Content,
			AuthorId = author.Id, CreatedDate = DateTimeOffset.UtcNow };
        await AddPostAsync(model);
		return model.Id;
	}

	public async Task UpdatePostAsync(PostModel updatedPost)
	{
		updatedPost.UpdatedDate = DateTimeOffset.UtcNow;
		_dbContext.Posts.Update(updatedPost);
		await _dbContext.SaveChangesAsync();
	}

	public async Task DeletePostAsync(PostModel model)
	{
		_dbContext.Posts.Remove(model);
		await _dbContext.SaveChangesAsync();
	}

	public bool IsOnFirstPage(int page)
	{
		return page == 1;
	}

	public bool IsOnLastPage(int page)
	{
		return page == Math.Ceiling(_dbContext.Posts.Count() / 5.0);
	}

	private IQueryable<PostModel> GetPostsWithTheirRelations()
	{
		return _dbContext.Posts.Include(p => p.Author);
	}

	private IQueryable<PostModel> GetPostsOrderedByCreationDate()
	{
		return GetPostsWithTheirRelations().OrderByDescending(p => p.CreatedDate);
	}

	private readonly DatabaseContext _dbContext;
	private const int POSTS_PER_PAGE = 5;
}
