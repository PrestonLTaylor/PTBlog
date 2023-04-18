using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTBlog.Data;
using PTBlog.Data.Repositories;
using PTBlog.Models;

namespace PTBlog.Controllers;

[Route("{controller}")]
public sealed class UsersController : Controller
{
	public UsersController(IUsersRepository usersRepository)
	{
		_usersRepository = usersRepository;
	}

	[Route("")]
	public async Task<IActionResult> Users()
	{
		var users = await _usersRepository.GetUsersAsync();
		return View(users);
	}

	[Route("{guid}")]
	public async Task<IActionResult> Profile(Guid? guid)
	{
		var user = await _usersRepository.GetUserByGuidAsync(guid);
		if (user is null)
		{
			return NotFound();
		}

		return View(user);
	}

	private readonly IUsersRepository _usersRepository;
}
