using Microsoft.AspNetCore.Mvc;
using Moq;
using PTBlog.Controllers;
using PTBlog.Data.Repositories;
using PTBlog.Models;

namespace PTBlogTests.Unit;

internal sealed class UsersControllerTest
{
	[Test]
	public async Task Users_ReturnsEveryUserInTheRepository()
	{
		// Arrange
		var fakeUsers = GenerateFakeUsers();
		_repoMock.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(fakeUsers);
		UsersController controller = new(_repoMock.Object);

		// Act
		var result = await controller.Users() as ViewResult;
		var modelUsers = result?.ViewData.Model as List<UserModel>;

		// Assert
		Assert.That(modelUsers, Has.Count.EqualTo(fakeUsers.Count));
	}

	[Test]
	public async Task Profile_ReturnsSpecifiedUser_WhenSuppliedId()
	{
		// Arrange
		var fakeUsers = GenerateFakeUsers();
		var specificUser = fakeUsers.First();
		_repoMock.Setup(repo => repo.GetUserByGuidAsync(specificUser.Id)).ReturnsAsync(specificUser);
		UsersController controller = new(_repoMock.Object);

		// Act
		var result = await controller.Profile(specificUser.Id) as ViewResult;
		var modelUser = result?.ViewData.Model as UserModel;

		// Assert
		Assert.That(modelUser, Is.EqualTo(specificUser));
	}

	[Test]
	public async Task Profile_ReturnsNotFound_WhenSuppliedInvalidId()
	{
		// Arrange
		var invalidUserGuid = Guid.NewGuid();
		_repoMock.Setup(repo => repo.GetUserByGuidAsync(invalidUserGuid)).ReturnsAsync((UserModel?)null);
		UsersController controller = new(_repoMock.Object);

		// Act
		var result = await controller.Profile(invalidUserGuid) as NotFoundResult;

		// Assert
		Assert.That(result, Is.Not.Null);
	}

	private List<UserModel> GenerateFakeUsers()
	{
		const int numberOfUsers = 10;
		var fakeUsers = new List<UserModel>();
		for (int i = 1; i < numberOfUsers; ++i)
		{
			var fakeUserString = $"Fake User {i}";
			fakeUsers.Add(new UserModel { Id = Guid.NewGuid(), ProfilePictureURL = fakeUserString, Username = fakeUserString});
		}

		return fakeUsers;
	}

	private readonly Mock<IUsersRepository> _repoMock = new();
}
