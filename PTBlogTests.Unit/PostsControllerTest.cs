using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PTBlog.Controllers;
using PTBlog.Data.Repositories;
using PTBlog.Models;
using System.Security.Claims;

namespace PTBlogTests.Unit;

internal class PostsControllerTest
{
	[Test]
	public async Task Listings_ReturnsEveryPostInTheRepository()
	{
		// Arrange
		var fakePosts = GenerateFakePosts();
		_postRepoMock.Setup(repo => repo.GetPostsAsync()).ReturnsAsync(fakePosts);
		PostsController controller = new(_postRepoMock.Object, _userRepoMock.Object);

		// Act
		var result = await controller.Listings() as ViewResult;
		var modelPosts = result?.ViewData.Model as List<PostModel>;

		// Assert
		Assert.That(modelPosts, Has.Count.EqualTo(fakePosts.Count));
	}

	[Test]
	public async Task Listing_ReturnsSpecifiedPost_WhenSuppliedId()
	{
		// Arrange
		var fakePosts = GenerateFakePosts();
		var specificPost = fakePosts.First();
		_postRepoMock.Setup(repo => repo.GetPostByIdAsync(specificPost.Id)).ReturnsAsync(specificPost);
		PostsController controller = new(_postRepoMock.Object, _userRepoMock.Object);

		// Act
		var result = await controller.Listing(specificPost.Id) as ViewResult;
		var modelPost = result?.ViewData.Model as PostModel;

		// Assert
		Assert.That(modelPost, Is.EqualTo(specificPost));
	}

	[Test]
	public async Task Listing_ReturnsNotFound_WhenSuppliedInvalidId()
	{
		// Arrange
		const int invalidPostId = -1;
		_postRepoMock.Setup(repo => repo.GetPostByIdAsync(invalidPostId)).ReturnsAsync((PostModel?)null);
		PostsController controller = new(_postRepoMock.Object, _userRepoMock.Object);

		// Act
		var result = await controller.Listing(invalidPostId) as NotFoundResult;

		// Assert
		Assert.That(result, Is.Not.Null);
	}

	[Test]
	public async Task Create_CreatesPost_WhenSuppliedValidPost()
	{
		// Arrange
		const string authorId = "Fake ID"; 
		UserModel fakeUser = new UserModel { Id = authorId, };
		_userRepoMock.Setup(repo => repo.GetUserByClaimAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(fakeUser);
		PostModel expectedPost = GeneratePostToCreate(fakeUser);
		_postRepoMock.Setup(repo => repo.AddPostAsync(It.Is<PostModel>(post => post.Equals(expectedPost)))).Verifiable();
		PostsController controller = new(_postRepoMock.Object, _userRepoMock.Object);

		// Act
		await controller.Create(new PostDTO(expectedPost.Title, expectedPost.Content));

		// Assert
		_postRepoMock.VerifyAll();
	}

	[Test]
	public async Task Delete_ShowsDeleteConfirmation_WhenSuppliedAValidPostIdAndIsAuthor()
	{
		// Arrange
		var fakePosts = GenerateFakePosts();
		var specificPost = fakePosts.First();
		_postRepoMock.Setup(repo => repo.GetPostByIdAsync(specificPost.Id)).ReturnsAsync(specificPost);
		_userRepoMock.Setup(repo => repo.GetUserByClaimAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new UserModel { Id = specificPost.AuthorId });
		PostsController controller = new(_postRepoMock.Object, _userRepoMock.Object);

		// Act
		var result = await controller.Delete(specificPost.Id) as ViewResult;
		var viewModel = result?.ViewData.Model as PostModel;

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(viewModel, Is.EqualTo(specificPost));
	}

	[Test]
	public async Task Delete_ReturnsNotFound_WhenPostDoesNotExist()
	{
		// Arrange
		const int invalidPostId = -1;
		_postRepoMock.Setup(repo => repo.GetPostByIdAsync(invalidPostId)).ReturnsAsync((PostModel?)null);
		PostsController controller = new(_postRepoMock.Object, _userRepoMock.Object);

		// Act
		var result = await controller.Delete(invalidPostId) as NotFoundResult;

		// Assert
		Assert.That(result, Is.Not.Null);
	}

	[Test]
	public async Task Delete_ReturnsForbid_WhenUserIsNotAuthor()
	{
		// Arrange
		string actualAuthorId = Guid.NewGuid().ToString();
		var fakePosts = GenerateFakePosts();
		var specificPost = fakePosts.First();
		_postRepoMock.Setup(repo => repo.GetPostByIdAsync(specificPost.Id)).ReturnsAsync(specificPost);
		_userRepoMock.Setup(repo => repo.GetUserByClaimAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new UserModel { Id = actualAuthorId });
		PostsController controller = new(_postRepoMock.Object, _userRepoMock.Object);

		// Act
		var result = await controller.Delete(specificPost.Id) as ForbidResult;

		// Assert
		Assert.That(result, Is.Not.Null);
	}

	[Test]
	public async Task DeleteConfirmed_DeletesAPost_WhenSuppliedAValidPostIdAndIsAuthor()
	{
		// Arrange
		var fakePosts = GenerateFakePosts();
		var specificPost = fakePosts.First();
		_postRepoMock.Setup(repo => repo.GetPostByIdAsync(specificPost.Id)).ReturnsAsync(specificPost);
		_postRepoMock.Setup(repo => repo.DeletePostAsync(It.Is<PostModel>(post => post.Equals(specificPost)))).Verifiable();
		_userRepoMock.Setup(repo => repo.GetUserByClaimAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new UserModel { Id = specificPost.AuthorId });
		PostsController controller = new(_postRepoMock.Object, _userRepoMock.Object);

		// Act
		var result = await controller.DeleteConfirmed(specificPost.Id) as RedirectToActionResult;

		// Assert
		Assert.That(result, Is.Not.Null);
		_postRepoMock.VerifyAll();
	}

	[Test]
	public async Task DeleteConfirmed_ReturnsNotFound_WhenPostDoesNotExist()
	{
		// Arrange
		const int invalidPostId = -1;
		_postRepoMock.Setup(repo => repo.GetPostByIdAsync(invalidPostId)).ReturnsAsync((PostModel?)null);
		PostsController controller = new(_postRepoMock.Object, _userRepoMock.Object);

		// Act
		var result = await controller.DeleteConfirmed(invalidPostId) as NotFoundResult;

		// Assert
		Assert.That(result, Is.Not.Null);
	}

	[Test]
	public async Task DeleteConfirmed_ReturnsForbid_WhenUserIsNotAuthor()
	{
		// Arrange
		string actualAuthorId = Guid.NewGuid().ToString();
		var fakePosts = GenerateFakePosts();
		var specificPost = fakePosts.First();
		_postRepoMock.Setup(repo => repo.GetPostByIdAsync(specificPost.Id)).ReturnsAsync(specificPost);
		_userRepoMock.Setup(repo => repo.GetUserByClaimAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new UserModel { Id = actualAuthorId });
		PostsController controller = new(_postRepoMock.Object, _userRepoMock.Object);

		// Act
		var result = await controller.DeleteConfirmed(specificPost.Id) as ForbidResult;

		// Assert
		Assert.That(result, Is.Not.Null);
	}

	private List<PostModel> GenerateFakePosts()
	{
		const int numberOfPosts = 10;
		var fakePosts = new List<PostModel>();
		for (int i = 1; i < numberOfPosts; ++i)
		{
			var fakePostString = $"Fake Post {i}";
			fakePosts.Add(new PostModel { Id = i, Content = fakePostString, Title = fakePostString });
		}

		return fakePosts;
	}

	private PostModel GeneratePostToCreate(UserModel fakeUser)
	{
		const int postId = 1;
		const string postTitle = "Fake Title";
		const string postContent = "Fake Content";
		return new PostModel { Id = postId, Title = postTitle, Content = postContent, AuthorId = fakeUser.Id };
	}

	private readonly Mock<IUsersRepository> _userRepoMock = new();
	private readonly Mock<IPostsRepository> _postRepoMock = new();
}
