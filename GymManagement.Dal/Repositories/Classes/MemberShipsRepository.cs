using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;

namespace GymManagement.DAL.Repositories.Classes
{
    public class MemberShipsRepository  :GenaricRepository<MemberShip> ,IMemberShipsRepository
    {
        private readonly GymDbContext _dbContext;
        public MemberShipsRepository(GymDbContext dbContext) : base(dbContext)    
        {
            _dbContext = dbContext;

        }
        public async Task<IEnumerable<MemberShip>> GetAllMemberShipsWithMemberAndPlanAsync(CancellationToken ct = default)
        {
            return await _dbContext.MemberShips
                .AsNoTracking()
                .Include(m => m.Member)
                .Include(m => m.Plan)
                .ToListAsync(ct);
        }
        public async Task<MemberShip?> GetActiveMembershipWithPlanAsync(int memberId,CancellationToken ct = default)
        {
            return await _dbContext.MemberShips
                .AsNoTracking()
                .Include(m => m.Plan)
                .Include(m => m.Member)
                .FirstOrDefaultAsync(
                    m => m.MemberId == memberId && m.EndDate > DateTime.Now,
                    ct);
        }

    }
}
