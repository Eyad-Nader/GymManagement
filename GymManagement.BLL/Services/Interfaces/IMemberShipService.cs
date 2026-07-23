using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MemberShipViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberShipService
    {
        Task<Result<IEnumerable<MemberShipViewModel>?>> GetAllMemberShipsAsync(CancellationToken ct = default);
        Task<Result> CreateMemberShipAsync(CreateMemberShipViewModel createMemberShip, CancellationToken ct = default);
        Task<Result> CancelMemberShipAsync(int id,CancellationToken ct = default);
        Task<Result<IEnumerable<MemberSelectViewModel>?>> GetMemberForDropdownAsync(CancellationToken ct = default);
        Task<Result<IEnumerable<PlanSelectViewModel>?>> GetPlanForDropdownAsync(CancellationToken ct = default);
    }
}
