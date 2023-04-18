using PTBlog.Models;

namespace PTBlog.Data.Repositories;

public interface IPostsRepository
{
	public Task<PostModel?> GetPostByIdAsync(int? postId);
}
