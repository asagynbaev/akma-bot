using Microsoft.EntityFrameworkCore;
using MindMate.Entities;

namespace MindMate
{
    // Контекст базы данных
    public class DialogContext : DbContext
    {   
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(DotNetEnv.Env.GetString("DB_CONNECTION_STRING"));
        }

        public DbSet<Dialog> Dialogs { get; set; }

        public DbSet<P2PUser> Users { get; set; }

        public DbSet<ErrorLogs> Errors { get; set; }

        public DbSet<Blacklist> Blacklists { get; set; }

        public DbSet<Whitelist> Whitelists { get; set; }
    }
}
