using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IMemberShipsRepository : IGenaricRepository<MemberShip>
    {
        Task<IEnumerable<MemberShip>> GetAllMemberShipsWithMemberAndPlanAsync(CancellationToken ct = default);
    }
}
