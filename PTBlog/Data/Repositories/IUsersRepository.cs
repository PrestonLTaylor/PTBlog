using PTBlog.Models;

namespace PTBlog.Data.Repositories;

public interface IUsersRepository
{
	public Task<List<UserModel>> GetUsersAsync();
	public Task<UserModel?> GetUserByIdAsync(string? id);
}
