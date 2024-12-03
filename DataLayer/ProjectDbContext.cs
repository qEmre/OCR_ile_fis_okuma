using Microsoft.EntityFrameworkCore;
using projectOCR.Models;

namespace projectOCR.DataLayer
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) 
        { 

        }
        public DbSet<Image> İmageTable { get; set; }
    }
}