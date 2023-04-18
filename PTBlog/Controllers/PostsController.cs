using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTBlog.Data;
using PTBlog.Models;

namespace PTBlog.Controllers;

[Route("{controller}")]
public sealed class PostsController : Controller
{
    public PostsController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Route("")]
    [Route("Listings")]
    public async Task<IActionResult> Listings()
    {
        var posts = await _dbContext.Posts.Include(p => p.Author).ToListAsync();
        return View(posts);
    }

	[Route("{id}")]
	public async Task<IActionResult> Listing(int? id)
    {
        var post = await FindListingByIdAsync(id);
        if (post is null)
        {
            return NotFound();
        }

        return View(post);
    }

    private async Task<PostModel?> FindListingByIdAsync(int? id)
    {
		if (id is null || _dbContext.Posts is null)
		{
			return null;
		}

		return await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
	}

    private readonly DatabaseContext _dbContext;
}
