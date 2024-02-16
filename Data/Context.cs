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
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<UserPoints> UserPoints { get; set; }
        public virtual DbSet<UserCupons> UserCupons { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }
    }
}
