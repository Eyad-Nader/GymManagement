using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class GenaricRepository<TEntity> : IGenaricRepository<TEntity> where TEntity : BaseEntity, new()
    {

        private readonly GymDbContext _context;
        private readonly DbSet<TEntity> _set;
        public GenaricRepository(GymDbContext context)
        {
            _context = context;
            _set = context.Set<TEntity>();
        }
        public void Add(TEntity entity)
        {
            _set.Add(entity);
        }

        public void Update(TEntity entity )
        {
            _set.Update(entity);
 
        }

        public void Delete(TEntity entity )
        {
            _set.Remove(entity);

        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query = tracking ? _set : _set.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _set.FindAsync(id, ct);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return _set.AsNoTracking().AnyAsync(predicate, ct);
        }

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return _set.AsNoTracking().FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate, CancellationToken ct = default)
        {
            if(predicate == null) 
                return await _set.AsNoTracking().CountAsync(ct);
            return await _set.AsNoTracking().CountAsync(predicate, ct);
        }
    }
}
