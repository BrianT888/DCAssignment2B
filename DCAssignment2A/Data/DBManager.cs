using DCAssignment2A.Models;
using Microsoft.EntityFrameworkCore;

namespace DCAssignment2A.Data
{
    public class DBManager : DbContext
    {
        //sets up connecton with db so don't need to do it manually in program.cs
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source = Database.db;");
        }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Set Cascade delete behaviour
            //user profile 1 to many bank accounts
            modelBuilder.Entity<BankAccount>()
                .HasOne<UserProfile>()
                .WithMany()
                .HasForeignKey(b => b.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            //bank account 1 to many transactions
            modelBuilder.Entity<Transaction>()
                .HasOne<BankAccount>()
                .WithMany()
                .HasForeignKey(t => t.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
