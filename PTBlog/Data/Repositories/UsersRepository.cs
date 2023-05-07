using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PTBlog.Claims;
using PTBlog.Models;
using System.Security.Claims;

namespace PTBlog.Data.Repositories;

public sealed class UsersRepository : IUsersRepository
{
	// DESIGN: Maybe move UserManager out of UserRepository and into its own Adapter
	public UsersRepository(DatabaseContext dbContext, UserManager<UserModel> userManager)
	{
		_dbContext = dbContext ?? throw new InvalidOperationException($"A null database context was provided to {nameof(UsersRepository)}");
		_userManager = userManager;
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

	public async Task<UserModel?> GetUserByClaimAsync(ClaimsPrincipal claim)
	{
		return await _userManager.GetUserAsync(claim);
	}

	private IQueryable<UserModel> GetUsersWithTheirRelations()
	{
		return _dbContext.Users.Include(u => u.Posts);
	}

	public async Task<bool> DoesClaimHaveAccessToPost(ClaimsPrincipal claim, PostModel post)
	{
		if (IsAdminRole.IsUserAnAdmin(claim))
		{
			return true;
		}

		var user = await GetUserByClaimAsync(claim);
		return user?.Id == post.AuthorId;
	}

	public async Task<string> GetRoleNameForUserAsync(UserModel user)
	{
		if (await _userManager.IsInRoleAsync(user, IsAdminRole.Name))
		{
			return "Admin";
		}

		return "User";
	}

	private readonly DatabaseContext _dbContext;
	private readonly UserManager<UserModel> _userManager;
}
