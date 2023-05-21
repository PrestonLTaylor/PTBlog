using PTBlog.Data;
using PTBlog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PTBlog.Installers;

public static class DbContextInstaller
{
	static public IServiceCollection AddPsqlDbContext(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetValue<string>("POSTGRESQLCONNSTR_DefaultConnection") ?? throw new InvalidOperationException("Connection string 'POSTGRESQLCONNSTR_DefaultConnection' not found.");
		services.AddDbContext<DatabaseContext>(options =>
		{
			options.UseNpgsql(connectionString);
		});

		services.AddDefaultIdentity<UserModel>().AddRoles<IdentityRole>().AddEntityFrameworkStores<DatabaseContext>();

		return services;
	}
}
