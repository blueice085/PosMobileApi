using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace PosMobileApi.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, Func<IQueryable<T>, IOrderedQueryable<T>> order = null);

        //Task<BaseIQueryResponse<T>> CreateQueryAlt(int pageIndex, int pageSize, IDictionary<string, string> filterColumns = null, List<ExtraFilter> extraFilters = null, string sortColumn = null, string sortOrder = null);

        //Task<BaseListResponse<T>> QueryListAsync(int pageIndex, int pageSize, string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null);

        //Task<BaseIQueryResponse<T>> CreateQueryAsync(int pageIndex, int pageSize, string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null);

        T Get(object id);

        Task<T> GetAsync(object id);

        T Single(Expression<Func<T, bool>> match, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);

        Task<T> SingleAsync(Expression<Func<T, bool>> match, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);

        T Add(T entity);

        Task<T> AddAsync(T entity);

        void Update(T entity);

        void Update(IEnumerable<T> entities);

        void Delete(T entity);

        void Delete(IEnumerable<T> entity);

        int Count(Expression<Func<T, bool>> predicate = null);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        bool Exist(Expression<Func<T, bool>> predicate);

        Task<bool> ExistAsync(Expression<Func<T, bool>> predicate);
    }

    public class ExtraFilter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Oprator { get; set; }
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbset;

        public Repository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbset = _context.Set<T>();
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null
            , Func<IQueryable<T>, IOrderedQueryable<T>> order = null)
        {
            IQueryable<T> query = _dbset;
            if (include != null)
                query = include(query);

            if (expression != null)
                query = query.Where(expression);

            //if (group != null)
            //    query = query.GroupBy(group);

            if (order != null)
                query = order(query);

            return query;
        }

        //public async Task<BaseListResponse<T>> QueryListAsync(int pageIndex, int pageSize, string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null)
        //{
        //    IQueryable<T> source = _dbset.AsNoTracking();
        //    if (!string.IsNullOrEmpty(filterQuery) && !string.IsNullOrEmpty(filterColumn) && IsValidProperty(filterColumn))
        //    {
        //        var filt = string.Format("{0}.Contains(@0,true)", filterColumn);
        //        source = source.Where(filt, filterQuery);
        //    }

        //    var count = await source.CountAsync();

        //    if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
        //    {
        //        sortOrder = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";

        //        source = source.OrderBy(string.Format("{0} {1}", sortColumn, sortOrder));
        //    }

        //    source = source
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize);

        //    var data = await source.ToListAsync();

        //    return new BaseListResponse<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        //}

        //public async Task<BaseIQueryResponse<T>> CreateQueryAlt(int pageIndex, int pageSize, IDictionary<string, string> filterColumns = null, List<ExtraFilter> extraFilters = null, string sortColumn = null, string sortOrder = null)
        //{
        //    IQueryable<T> source = _dbset.AsNoTracking();

        //    if (filterColumns != null && filterColumns.Count > 0)
        //    {
        //        var filt = "";
        //        var objs = new List<object>();
        //        var i = 0;
        //        foreach (KeyValuePair<string, string> ele in filterColumns)
        //        {
        //            if (!string.IsNullOrEmpty(ele.Value) && !string.IsNullOrEmpty(ele.Key) && IsValidProperty(ele.Key))
        //            {
        //                if (!string.IsNullOrEmpty(filt))
        //                {
        //                    filt += " OR ";
        //                }

        //                //filt += string.Format($"DynamicFunctions.Like({ele.Key}, \"%@{i}%\")");
        //                filt += string.Format($"{ele.Key}.ToLower().Contains(@{i})");
        //                objs.Add(ele.Value.ToLower());
        //                i++;
        //            }
        //        }
        //        if (objs.Count > 0)
        //        {
        //            source = source.Where(filt, objs.ToArray());
        //        }
        //    }

        //    if (extraFilters != null && extraFilters.Count > 0)
        //    {
        //        var filt = "";
        //        var objs = new List<object>();
        //        var i = 0;
        //        foreach (var ele in extraFilters)
        //        {
        //            if (!string.IsNullOrEmpty(ele.Value) && !string.IsNullOrEmpty(ele.Name) && IsValidProperty(ele.Name))
        //            {
        //                if (!string.IsNullOrEmpty(filt))
        //                {
        //                    filt += " AND ";
        //                }

        //                filt += string.Format($"{ele.Name} {ele.Oprator} @{i}");
        //                objs.Add(ele.Value);
        //                i++;
        //            }
        //        }
        //        if (objs.Count > 0)
        //        {
        //            source = source.Where(filt, objs.ToArray());
        //        }
        //    }

        //    var count = await source.CountAsync();

        //    if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
        //    {
        //        sortOrder = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";

        //        source = source.OrderBy(string.Format("{0} {1}", sortColumn, sortOrder));
        //    }

        //    source = source
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize);

        //    return new BaseIQueryResponse<T>(source, count, pageIndex, pageSize);
        //}

        //public async Task<BaseIQueryResponse<T>> CreateQueryAsync(int pageIndex, int pageSize, string sortColumn = null, string sortOrder = null, string filterColumn = null, string filterQuery = null)
        //{
        //    IQueryable<T> source = _dbset.AsNoTracking();
        //    if (!string.IsNullOrEmpty(filterQuery) && !string.IsNullOrEmpty(filterColumn) && IsValidProperty(filterColumn))
        //    {
        //        var filt = string.Format("{0}.StartsWith(@0)", filterColumn);
        //        source = source.Where(filt, filterQuery);
        //    }

        //    var count = await source.CountAsync();

        //    if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
        //    {
        //        sortOrder = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";

        //        source = source.OrderBy(string.Format("{0} {1}", sortColumn, sortOrder));
        //    }

        //    source = source
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize);

        //    return new BaseIQueryResponse<T>(source, count, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        //}

        private bool IsValidProperty(string propertyName, bool throwExceptionIfNotFound = true)
        {
            var prop = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (prop == null && throwExceptionIfNotFound)
                throw new NotSupportedException(string.Format($"Error: Property '{propertyName}' does not exist."));

            return prop != null;
        }

        public T Get(object id)
        {
            return _dbset.Find(id);
        }

        public async Task<T> GetAsync(object id)
        {
            return await _dbset.FindAsync(id);
        }

        public T Single(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            IQueryable<T> query = _dbset;
            if (includes != null)
                query = includes(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return orderBy(query).AsNoTracking().FirstOrDefault();

            return query.AsNoTracking().FirstOrDefault();
        }

        public async Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            IQueryable<T> query = _dbset;
            if (includes != null)
                query = includes(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).AsNoTracking().FirstOrDefaultAsync();

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public int Count(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return _dbset.AsNoTracking().Count();
            }
            return _dbset.Where(predicate).AsNoTracking().Count();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return await _dbset.AsNoTracking().CountAsync();
            }
            return await _dbset.Where(predicate).AsNoTracking().CountAsync();
        }

        public bool Exist(Expression<Func<T, bool>> predicate)
        {
            var exist = _dbset.Where(predicate);
            return exist.AsNoTracking().Any();
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> predicate)
        {
            var exist = _dbset.Where(predicate);
            return await exist.AsNoTracking().AnyAsync();
        }

        public T Add(T entity)
        {
            return _dbset.Add(entity).Entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            var result = await _dbset.AddAsync(entity);
            return result.Entity;
        }

        public void Add(IEnumerable<T> entities)
        {
            _dbset.AddRange(entities);
        }

        public async Task AddAsync(IEnumerable<T> entities)
        {
            await _dbset.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbset.Update(entity);
        }

        public void Update(IEnumerable<T> entities)
        {
            _dbset.UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            _dbset.RemoveRange(entities);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
