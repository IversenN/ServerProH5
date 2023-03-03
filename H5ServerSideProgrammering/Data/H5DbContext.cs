using H5ServerSideProgrammering.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace H5ServerSideProgrammering.Data
{
    public class H5DbContext : DbContext
    {
        public H5DbContext() {}
        public H5DbContext(DbContextOptions<H5DbContext> options): base(options) {}
        public DbSet<Login> Login { get; set; }
        public DbSet<TodoItem> TodoItem { get; set; }
    }
}