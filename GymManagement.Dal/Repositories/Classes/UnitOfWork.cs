using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _context;
        private readonly Dictionary<string, object> _repositories = [];

        public UnitOfWork(GymDbContext context, ISessionRepository sessionRepository)
        {
            _context = context;
            SessionRepository = sessionRepository;
        }

        public ISessionRepository SessionRepository { get; }

        public IGenaricRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;
            if(!_repositories.TryGetValue(typeName, out object? repositoryInstance))
            {
                repositoryInstance = new GenaricRepository<TEntity>(_context);
                _repositories.Add(typeName, repositoryInstance);
            }
            return (IGenaricRepository<TEntity>)repositoryInstance;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct) => await _context.SaveChangesAsync(ct);
    }
}
