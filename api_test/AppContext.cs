using Microsoft.EntityFrameworkCore;

namespace api_test
{
    public class AppContext: DbContext
    {
        public DbSet<Data> Data { get; set; } = null!;

        public AppContext(DbContextOptions<AppContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
