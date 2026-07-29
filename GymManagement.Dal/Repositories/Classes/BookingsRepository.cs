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
    public class BookingsRepository :GenaricRepository<Booking> ,IBookingsRepository
    {
        private readonly GymDbContext _dbContext;
        public BookingsRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }
        public async Task<IEnumerable<Booking>> GetAllBookingForSession(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Bookings
                          .AsNoTracking()
                          .Include(B=>B.Member)
                          .Where(B=>B.SessionId == sessionId)
                          .ToListAsync(ct);
        }
        public async Task<List<Member>> GetMembersNotBookedInSession(int sessionId,CancellationToken ct = default)
        {
            return await _dbContext.Members
                .Where(m => !m.Bookings.Any(b => b.SessionId == sessionId))
                .ToListAsync(ct);
        }
    
    }
    
}
