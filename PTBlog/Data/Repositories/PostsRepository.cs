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

	public async Task<PostModel?> GetPostByIdAsync(int? postId)
	{
		if (postId is null)
		{
			return null;
		}

		return await GetPostsWithTheirRelations().FirstOrDefaultAsync(p => p.Id == postId);
	}

	private IQueryable<PostModel> GetPostsWithTheirRelations()
	{
		return _dbContext.Posts.Include(p => p.Author);
	}

	private readonly DatabaseContext _dbContext;
}
