using Microsoft.EntityFrameworkCore;

namespace MindMate
{
    // Контекст базы данных
    public class DialogContext : DbContext
    {   
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(DotNetEnv.Env.GetString("DB_CONNECTION_STRING"));
        }

        public DbSet<Dialog>? Dialogs { get; set; }
    }
}
