using Markdig;
using Westwind.AspNetCore.Markdown;

namespace PTBlog.Installers;

public static class MarkdownInstaller
{
	static public IServiceCollection AddMarkdownServices(this IServiceCollection services)
	{
		services.AddMarkdown(options =>
		{
			options.ConfigureMarkdigPipeline = builder => { builder.DisableHtml(); };
		});

		return services;
	}
}
