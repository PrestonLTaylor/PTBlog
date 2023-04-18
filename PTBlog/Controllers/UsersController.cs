using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTBlog.Data;
using PTBlog.Models;

namespace PTBlog.Controllers;

[Route("{controller}")]
public sealed class UsersController : Controller
{
	public UsersController(DatabaseContext dbContext)
	{
		_dbContext = dbContext;
	}

	[Route("")]
	public async Task<IActionResult> Users()
	{
		var users = await _dbContext.Users.ToListAsync();
		return View(users);
	}

	[Route("{guid}")]
	public async Task<IActionResult> Profile(Guid? guid)
	{
		var user = await GetUserByGuidAsync(guid);
		if (user is null)
		{
			return NotFound();
		}

		return View(user);
	}

	private async Task<UserModel?> GetUserByGuidAsync(Guid? guid)
	{
		if (guid is null || _dbContext.Users is null)
		{
			return null;
		}

		return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(guid));
	}

	private readonly DatabaseContext _dbContext;
}
