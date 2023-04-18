namespace PTBlog.Data.Repositories;

public static class RepositoriesExtensions
{
	static public IServiceCollection AddRepositoriesSerivces(this IServiceCollection services)
	{
		services.AddTransient<IPostsRepository, PostsRepository>();
		services.AddTransient<IUsersRepository, UsersRepository>();
		return services;
	}
}
