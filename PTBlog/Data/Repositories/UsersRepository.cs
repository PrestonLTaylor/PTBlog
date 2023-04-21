using Microsoft.EntityFrameworkCore;
using PTBlog.Models;

namespace PTBlog.Data.Repositories;

public sealed class UsersRepository : IUsersRepository
{
	public UsersRepository(DatabaseContext dbContext)
	{
		_dbContext = dbContext ?? throw new InvalidOperationException($"A null database context was provided to {nameof(UsersRepository)}");
	}

	public async Task<List<UserModel>> GetUsersAsync()
	{
		return await GetUsersWithTheirRelations().ToListAsync();
	}

	public async Task<UserModel?> GetUserByIdAsync(string? id)
	{
		if (id is null)
		{
			return null;
		}

		return await GetUsersWithTheirRelations().FirstOrDefaultAsync(u => u.Id.Equals(id));
	}

	private IQueryable<UserModel> GetUsersWithTheirRelations()
	{
		return _dbContext.Users.Include(u => u.Posts);
	}

	private readonly DatabaseContext _dbContext;
}
