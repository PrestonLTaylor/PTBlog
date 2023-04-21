using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PTBlog.Models;

namespace PTBlog.Data;

public sealed class DatabaseContext : IdentityDbContext<UserModel>
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

    public DbSet<PostModel> Posts { get; set; }
}
