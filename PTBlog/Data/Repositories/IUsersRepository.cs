using PTBlog.Models;
using System.Security.Claims;

namespace PTBlog.Data.Repositories;

public interface IUsersRepository
{
	public Task<List<UserModel>> GetUsersAsync();
	public Task<UserModel?> GetUserByIdAsync(string? id);
	public Task<UserModel?> GetUserByClaimAsync(ClaimsPrincipal claim);
}
