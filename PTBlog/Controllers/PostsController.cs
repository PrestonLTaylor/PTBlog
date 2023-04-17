using Microsoft.AspNetCore.Mvc;
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
    public IActionResult Listings()
    {
        var posts = _dbContext.Posts.ToList();
        return View(posts);
    }

    private readonly DatabaseContext _dbContext;
}
