using Serilog;

namespace PTBlog.Installers;

public static class SerilogInstaller
{
	static public IHostBuilder UseSerilogWithConfiguration(this IHostBuilder host, IConfiguration configuration)
	{
		host.UseSerilog((context, config) =>
		{
			config.ReadFrom.Configuration(configuration);
		});

		return host;
	}
}
