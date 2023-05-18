using PTBlog.Models;

namespace PTBlog.Data.Repositories;

public interface IPostsRepository
{
	public Task<List<PostModel>> GetPostsAsync();
	public Task<List<PostModel>> GetPostsOnPageAsync(int page);
	public Task<PostModel?> GetLatestPostAsync();
    public Task<List<PostModel>> GetPostsByTitleAsync(string titleWanted);
	public Task<PostModel?> GetPostByIdAsync(int? postId);

	public Task AddPostAsync(PostModel post);
	public Task<int> AddPostFromDTOAsync(PostDTO postDto, UserModel author);

	public Task UpdatePostAsync(PostModel post);

	public Task DeletePostAsync(PostModel post);
}
