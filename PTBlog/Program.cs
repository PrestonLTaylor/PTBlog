using Serilog;
using PTBlog.Data;
using PTBlog.Models;
using Microsoft.EntityFrameworkCore;
using PTBlog.Data.Repositories;
using Westwind.AspNetCore.Markdown;
using Markdig;
using Microsoft.AspNetCore.Identity;
using PTBlog.Endpoints.V1;
using Microsoft.OpenApi.Models;
using PTBlog.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(builder.Configuration);
});

var connectionString = builder.Configuration.GetValue<string>("POSTGRESQLCONNSTR_DefaultConnection") ?? throw new InvalidOperationException("Connection string 'POSTGRESQLCONNSTR_DefaultConnection' not found.");
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddDefaultIdentity<UserModel>().AddRoles<IdentityRole>().AddEntityFrameworkStores<DatabaseContext>();

builder.Services.AddRepositoriesSerivces();
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddMarkdown(options =>
{
    options.ConfigureMarkdigPipeline = builder => { builder.DisableHtml(); };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen((options) =>
{
	options.SwaggerDoc(APIRoutes.Version, new OpenApiInfo { Title = $"PTBlog API {APIRoutes.Version}", Version = APIRoutes.Version });
});

var app = builder.Build();

await app.UseDatabaseSeedingAsync(app.Environment.IsDevelopment());

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMarkdown();

var swaggerConfiguration = SwaggerConfiguration.From(app.Configuration) ?? throw new InvalidOperationException($"Swagger configuration not found.");
app.UseSwagger((options) => { options.RouteTemplate = swaggerConfiguration.RouteTemplate; });
app.UseSwaggerUI((options) => { options.SwaggerEndpoint(swaggerConfiguration.Endpoint, swaggerConfiguration.Description); });

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapPostsApiEndpoints();

app.Run();