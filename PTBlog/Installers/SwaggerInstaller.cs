using Microsoft.OpenApi.Models;
using PTBlog.Configuration;
using PTBlog.Endpoints.V1;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PTBlog.Installers;

public static class SwaggerInstaller
{
	static public IServiceCollection AddSwaggerServices(this IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen((options) =>
		{
			options.SwaggerDoc(APIRoutes.Version, new OpenApiInfo { Title = $"PTBlog API {APIRoutes.Version}", Version = APIRoutes.Version });
			options.AddApiKeySecurityDefinition();
		});

		return services;
	}

	static private void AddApiKeySecurityDefinition(this SwaggerGenOptions options)
	{
		const string apiKeyId = "API Key";
		options.AddSecurityDefinition(apiKeyId, new OpenApiSecurityScheme
		{
			Name = "API_KEY",
			Description = "API Key authorization in request headers",
			In = ParameterLocation.Header,
			Type = SecuritySchemeType.ApiKey,
		});

		options.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme()
		{
			Reference = new OpenApiReference
			{
				Type = ReferenceType.SecurityScheme,
				Id = apiKeyId,
			}
		}, new List<string>() } });
	}

	static public WebApplication UseSwaggerAndUI(this WebApplication app)
	{
		var swaggerConfiguration = SwaggerConfiguration.From(app.Configuration) ?? throw new InvalidOperationException($"Swagger configuration not found.");
		app.UseSwagger((options) => { options.RouteTemplate = swaggerConfiguration.RouteTemplate; });
		app.UseSwaggerUI((options) => { options.SwaggerEndpoint(swaggerConfiguration.Endpoint, swaggerConfiguration.Description); });
		return app;
	}
}
