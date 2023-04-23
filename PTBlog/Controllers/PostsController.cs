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

        var post = await CreatePostFromDTOAsync(postDto);
        await _postsRepository.AddPostAsync(post);
        return RedirectToListing(post.Id);
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
		if (await IsNotUsersPostAsync(post))
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
		if (await IsNotUsersPostAsync(post))
		{
			return Forbid();
		}

        UpdatePostTitleAndContent(post, postDto);
		AddUpdatedDateToPost(post);
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
        if (await IsNotUsersPostAsync(post))
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
		if (await IsNotUsersPostAsync(post))
		{
			return Forbid();
		}

        await _postsRepository.DeletePostAsync(post);
        return RedirectToListings();
	}

    private async Task<PostModel> CreatePostFromDTOAsync(PostDTO postDto)
    {
        var post = new PostModel { Title = postDto.Title, Content = postDto.Content };

		await AddAuthorToPostAsync(post);
		AddCurrentDateToPost(post);

        return post;
	}

    private async Task AddAuthorToPostAsync(PostModel post)
    {
        var user = await _usersRepository.GetUserByClaimAsync(User);
		post.AuthorId = user!.Id;
    }

    private void AddCurrentDateToPost(PostModel post)
    {
        post.CreatedDate = DateTimeOffset.UtcNow;
    }

    private void AddUpdatedDateToPost(PostModel post)
    {
        post.UpdatedDate = DateTimeOffset.UtcNow;
    }

    private void UpdatePostTitleAndContent(PostModel post, PostDTO postDto)
    {
        post.Title = postDto.Title;
		post.Content = postDto.Content;
	}

	private async Task<bool> IsNotUsersPostAsync(PostModel post)
    {
        var user = await _usersRepository.GetUserByClaimAsync(User);
        return post.AuthorId != user!.Id;
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
