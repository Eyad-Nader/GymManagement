using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenaricRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCatrgoryAsync(CancellationToken ct = default);
        Task<Session?> GetSessionWithTrainerAndCategoryByIdAsync(int Id, CancellationToken ct = default);

        Task<int> GetCountOfBookedSlotsAsync(int Id, CancellationToken ct = default);
    }
}
