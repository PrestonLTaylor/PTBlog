using PTBlog.Models;
using System.Security.Claims;

namespace PTBlog.Data.Repositories;

public interface IUsersRepository
{
	public Task<List<UserModel>> GetUsersAsync();
	public Task<UserModel?> GetUserByIdAsync(string? id);
	public Task<UserModel?> GetUserFromRequestAsync(HttpRequest request);
	public Task<UserModel?> GetUserByClaimAsync(ClaimsPrincipal claim);

	public Task<bool> DoesUserHaveAccessToPost(UserModel user, PostModel post);

	public Task<string> GetRoleNameForUserAsync(UserModel user);

	public Task<string> GenerateNewApiKeyForUserAsync(UserModel user);
	public Task RevokeApiKeyForUserAsync(UserModel user);
}
