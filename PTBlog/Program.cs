using PTBlog.Data;
using PTBlog.Models;
using Microsoft.EntityFrameworkCore;
using PTBlog.Data.Repositories;
using Westwind.AspNetCore.Markdown;
using Markdig;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddDefaultIdentity<UserModel>().AddEntityFrameworkStores<DatabaseContext>();

builder.Services.AddRepositoriesSerivces();
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddMarkdown(options =>
{
    options.ConfigureMarkdigPipeline = builder => { builder.DisableHtml(); };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.UseDatabaseSeedingAsync();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMarkdown();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
