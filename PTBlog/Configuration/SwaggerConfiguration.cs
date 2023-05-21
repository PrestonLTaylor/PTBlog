namespace PTBlog.Configuration;

public sealed class SwaggerConfiguration
{
	static public SwaggerConfiguration? From(IConfiguration configuration)
	{
		const string SectionName = "Swagger";
		SwaggerConfiguration? value = new();
		configuration.GetSection(SectionName).Bind(value);
		return value;
	}

	public string RouteTemplate { get; set; }
	public string Description { get; set; }
	public string Endpoint { get; set; }
}