using Microsoft.EntityFrameworkCore;
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
		return await GetPostsWithTheirRelations().ToListAsync();
	}

	public async Task<List<PostModel>> GetPostsByTitleAsync(string wantedTitle)
	{
		return await GetPostsWithTheirRelations().Where(x => x.Title.Contains(wantedTitle)).ToListAsync();
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

	public async Task UpdatePostAsync(PostModel updatedPost)
	{
		_dbContext.Posts.Update(updatedPost);
		await _dbContext.SaveChangesAsync();
	}

	public async Task DeletePostAsync(PostModel model)
	{
		_dbContext.Posts.Remove(model);
		await _dbContext.SaveChangesAsync();
	}

	private IQueryable<PostModel> GetPostsWithTheirRelations()
	{
		return _dbContext.Posts.Include(p => p.Author);
	}

	private readonly DatabaseContext _dbContext;
}
