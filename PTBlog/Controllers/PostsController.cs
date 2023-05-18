using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PTBlog.Data.Repositories;
using PTBlog.Models;

namespace PTBlog.Controllers;

[Route("{controller}")]
public sealed class PostsController : Controller
{
    public PostsController(IPostsRepository postsRepository, IUsersRepository usersRepository)
    {
        _postsRepository = postsRepository;
        _usersRepository = usersRepository;
    }

    [Route("")]
    [Route("{action}/{page}")]
    public async Task<IActionResult> Listings(int page = 1)
    {
        if (page < 1)
        {
            return BadRequest();
        }

        var posts = await _postsRepository.GetPostsOnPageAsync(page);
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

    [Route("{action}")]
    public async Task<IActionResult> Search(BlogSearchDTO blogSearch)
    {
        var posts = await _postsRepository.GetPostsByTitleAsync(blogSearch.TitleWanted);
        ViewData["TitleWanted"] = blogSearch.TitleWanted;
        return View(posts);
    }

    [Route("{action}")]
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("{action}")]
    [Authorize]
    public async Task<IActionResult> Create([Bind("Title,Content")]PostDTO postDto)
    {
        if (!ModelState.IsValid)
        {
            return View(postDto);
        }

        var author = await _usersRepository.GetUserByClaimAsync(User);
        var postId = await _postsRepository.AddPostFromDTOAsync(postDto, author!);
        return RedirectToListing(postId);
    }

    [Route("{action}/{id}")]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _postsRepository.GetPostByIdAsync(id);
		if (post is null)
		{
			return NotFound();
		}
		if (!await DoesUserHaveAccessToPost(post))
		{
			return Forbid();
		}

		return View(new PostDTO{ Title = post.Title, Content = post.Content });
    }

    [HttpPost("Edit/{id}")]
    [Authorize]
    public async Task<IActionResult> EditConfirmed(int id, [Bind("Title,Content")]PostDTO postDto)
    {
		if (!ModelState.IsValid)
		{
			return View(postDto);
		}

		var post = await _postsRepository.GetPostByIdAsync(id);
        if (post is null)
        {
            return NotFound();
        }
		if (!await DoesUserHaveAccessToPost(post))
		{
			return Forbid();
		}

        UpdatePostTitleAndContent(post, postDto);
        await _postsRepository.UpdatePostAsync(post);
        return RedirectToListing(id);
    }

    [Route("{action}/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _postsRepository.GetPostByIdAsync(id);
        if (post is null)
        {
            return NotFound();
        }
        if (!await DoesUserHaveAccessToPost(post))
        {
            return Forbid();
        }

        return View(post);
    }

    [HttpPost("Delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
		var post = await _postsRepository.GetPostByIdAsync(id);
		if (post is null)
		{
			return NotFound();
		}
		if (!await DoesUserHaveAccessToPost(post))
		{
			return Forbid();
		}

        await _postsRepository.DeletePostAsync(post);
        return RedirectToListings();
	}

    private void UpdatePostTitleAndContent(PostModel post, PostDTO postDto)
    {
        post.Title = postDto.Title;
		post.Content = postDto.Content;
	}

	private async Task<bool> DoesUserHaveAccessToPost(PostModel post)
    {
        var user = await _usersRepository.GetUserByClaimAsync(User);
        if (user is null)
        {
            return false;
        }

        return await _usersRepository.DoesUserHaveAccessToPost(user, post);
	}

    private IActionResult RedirectToListings()
    {
		return RedirectToAction(nameof(Listings));
	}

	private IActionResult RedirectToListing(int postId)
	{
		return RedirectToAction(nameof(Listing), new { id = postId });
	}

	private readonly IPostsRepository _postsRepository;
	private readonly IUsersRepository _usersRepository;
}
