using Microsoft.EntityFrameworkCore;

namespace PosMobileApi.Data
{
    public interface IUow
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        int SaveChanges();

        Task<int> SaveChangesAsync();
    }

    public interface IUow<TContext> : IUow where TContext : DbContext
    {
        TContext Context { get; }
    }

    public class Uow<T> : IUow<T>, IUow where T : DbContext, IDisposable
    {
        private Dictionary<Type, object> _repositories;

        public Uow(T context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public T Context { get; }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<TEntity>(Context);
            }
            return (IRepository<TEntity>)_repositories[type];
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
