using PosMobileApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace PosMobileApi.Data
{
    public partial class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }      
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserOtpCode> UserOtpCodes { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }
    }
}
