namespace PTBlog.Data.Repositories;

public static class RepositoriesExtensions
{
	static public IServiceCollection AddRepositoriesSerivces(this IServiceCollection services)
	{
		services.AddTransient<IPostsRepository, PostsRepository>();
		return services;
	}
}
