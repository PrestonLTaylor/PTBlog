using Microsoft.AspNetCore.Mvc;
using PTBlog.Data.Repositories;

namespace PTBlog.Controllers;

[Route("{controller}")]
public sealed class PostsController : Controller
{
    public PostsController(IPostsRepository postsRepository)
    {
		_postsRepository = postsRepository;
    }

    [Route("")]
    [Route("Listings")]
    public async Task<IActionResult> Listings()
    {
        var posts = await _postsRepository.GetPostsAsync();
        return View(posts);
    }

	[Route("{id}")]
	public async Task<IActionResult> Listing(int? id)
    {
        var post = await _postsRepository.GetPostByIdAsync(id);
        if (post is null)
        {
            return NotFound();
        }

        return View(post);
    }

    private readonly IPostsRepository _postsRepository;
}
