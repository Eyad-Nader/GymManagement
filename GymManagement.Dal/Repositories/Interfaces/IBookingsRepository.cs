using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IBookingsRepository : IGenaricRepository<Booking> 
    {
        Task<IEnumerable<Booking>> GetAllBookingForSession(int sessionId, CancellationToken ct = default);
        Task<List<Member>> GetMembersNotBookedInSession(int sessionId ,CancellationToken ct = default);
    }
}
