using PTBlog.Models;

namespace PTBlog.Data.Repositories;

public interface IPostsRepository
{
	public Task<List<PostModel>> GetPostsAsync();
	public Task<PostModel?> GetPostByIdAsync(int? postId);

	public Task AddPostAsync(PostModel post);
}
