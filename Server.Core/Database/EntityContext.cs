namespace Trellheim.Server.Core.Database
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Data.Client;

    public interface IEntityContext : IDisposable
    {
        IDbSet<T> EntitySet<T>() where T : class;
        int SaveChanges();
        Task<int> SaveChangesAsync();
        DbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
        DbContextTransaction BeginTransaction();
    }

    public class EntityContext : DbContext, IEntityContext
    {
        public EntityContext()
            : base("Name=MainGame")
        {
        }

        public EntityContext(DbConnection connection)
            : base(connection, true)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("MainGame");
            modelBuilder.Entity<Account>().ToTable("Accounts");
            modelBuilder.Configurations.AddFromAssembly(typeof(IEntityContext).Assembly);
            // You can also override/add conventions here by accessing modelBuilder.Conventions
        }

        public IDbSet<T> EntitySet<T>() where T : class
        {
            return Set<T>();
        }

        public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return BeginTransaction(isolationLevel);
        }

        public DbContextTransaction BeginTransaction()
        {
            return BeginTransaction();
        }
    }
}
