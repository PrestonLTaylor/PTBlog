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

	[Route("{id:guid}")]
	public async Task<IActionResult> Profile(string id)
	{
		var user = await _usersRepository.GetUserByIdAsync(id);
		if (user is null)
		{
			return NotFound();
		}

		return View(user);
	}

	private readonly IUsersRepository _usersRepository;
}
