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

    private readonly DatabaseContext _dbContext;
}
