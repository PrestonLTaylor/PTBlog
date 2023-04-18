using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTBlog.Data;

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
        if (id is null || _dbContext.Posts is null)
        {
            return NotFound();
        }

        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }

        return View(post);
    }

    private readonly DatabaseContext _dbContext;
}
