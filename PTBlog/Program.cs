using PTBlog.Data;
using PTBlog.Data.Repositories;
using Westwind.AspNetCore.Markdown;
using PTBlog.Endpoints.V1;
using PTBlog.Installers;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilogWithConfiguration(builder.Configuration);

builder.Services.AddPsqlDbContext(builder.Configuration);

builder.Services.AddRepositoriesSerivces();
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddMarkdownServices();

builder.Services.AddSwaggerServices();

var app = builder.Build();

await app.UseDatabaseSeedingAsync(app.Environment.IsDevelopment());

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMarkdown();

app.UseSwaggerAndUI();

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