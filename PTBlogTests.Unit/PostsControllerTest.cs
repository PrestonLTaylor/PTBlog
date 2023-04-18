using Microsoft.AspNetCore.Mvc;
using Moq;
using PTBlog.Controllers;
using PTBlog.Data.Repositories;
using PTBlog.Models;

namespace PTBlogTests.Unit;

internal class PostsControllerTest
{
	[Test]
	public async Task Listings_ReturnsEveryPostInTheRepository()
	{
		// Arrange
		var fakePosts = GenerateFakePosts();
		_repoMock.Setup(repo => repo.GetPostsAsync()).ReturnsAsync(fakePosts);
		PostsController controller = new(_repoMock.Object);

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
		_repoMock.Setup(repo => repo.GetPostByIdAsync(specificPost.Id)).ReturnsAsync(specificPost);
		PostsController controller = new(_repoMock.Object);

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
		_repoMock.Setup(repo => repo.GetPostByIdAsync(invalidPostId)).ReturnsAsync((PostModel?)null);
		PostsController controller = new(_repoMock.Object);

		// Act
		var result = await controller.Listing(invalidPostId) as NotFoundResult;

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

	private readonly Mock<IPostsRepository> _repoMock = new();
}
