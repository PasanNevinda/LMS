using LMS.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{

    // do not use this Repository pattern for user tables, just use " private readonly UserManager<ApplicationUser> _userManager; "
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext _context { get; set; }
        private DbSet<T> _dbSet { get; set; }

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            T? entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id, QueryOptions<T> options)
        {
            IQueryable<T> query = _dbSet;
            if (options.HasWhere)
            {
                query = query.Where(options.Where);
            }
            if (options.HasOrderBy)
            {
                query = query.OrderBy(options.OrderBy);
            }
            foreach (var include in options.GetIncludes())
            {
                query = query.Include(include);
            }

            var key = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.FirstOrDefault();
            string primaryKeyName = key?.Name;
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, primaryKeyName) == id);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        // Tkey for generic type support
        // propertyName for get from any table column
        public async Task<IEnumerable<T>> GetAllByIdAsync<TKey>(TKey id, string propertyName, QueryOptions<T> options)
        {
            IQueryable<T> query = _dbSet;
            if (options.HasWhere)
            {
                query = query.Where(options.Where);
            }
            if (options.HasOrderBy)
            {
                query = query.OrderBy(options.OrderBy);
            }

            foreach (string include in options.GetIncludes())
            {
                query = query.Include(include);
            }

            // Filter by the specified property name and id
            query = query.Where(e => EF.Property<TKey>(e, propertyName).Equals(id));

            return await query.ToListAsync();

        }
    }
}
