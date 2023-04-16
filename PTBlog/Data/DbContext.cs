using Microsoft.EntityFrameworkCore;
using PTBlog.Models;

namespace PTBlog.Data;

public sealed class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}
}
