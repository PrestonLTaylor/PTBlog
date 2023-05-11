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

	public async Task<UserModel?> GetUserFromRequestAsync(HttpRequest request)
	{
		var apiKey = GetApiKeyFromRequest(request);
		return await GetUsersWithTheirRelations().FirstOrDefaultAsync(u => u.ApiKey != null && u.ApiKey.Equals(apiKey));
	}

	private string GetApiKeyFromRequest(HttpRequest request)
	{
		return request.Headers["API_KEY"].ToString();
	}

	public async Task<UserModel?> GetUserByClaimAsync(ClaimsPrincipal claim)
	{
		return await _userManager.GetUserAsync(claim);
	}

	private IQueryable<UserModel> GetUsersWithTheirRelations()
	{
		return _dbContext.Users.Include(u => u.Posts);
	}

	public async Task<bool> DoesUserHaveAccessToPost(UserModel user, PostModel post)
	{
		if (await _userManager.IsInRoleAsync(user, IsAdminRole.Name))
		{
			return true;
		}

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

	public async Task<string> GenerateNewApiKeyForUserAsync(UserModel user)
	{
		var apiKey = Guid.NewGuid().ToString();
		await ChangeApiKeyForUserAsync(user, apiKey);
		return apiKey;
	}

	public async Task RevokeApiKeyForUserAsync(UserModel user)
	{
		await ChangeApiKeyForUserAsync(user, null);
    }

	private async Task ChangeApiKeyForUserAsync(UserModel user, string? newApiKey)
	{
        user.ApiKey = newApiKey;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    private readonly DatabaseContext _dbContext;
	private readonly UserManager<UserModel> _userManager;
}
