using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.Common;
using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<Result<IEnumerable<MemberViewModel>>> GetAllMembersAsync(CancellationToken ct = default);
        Task<Result<MemberViewModel?>> GetMemberByIdAsync(int id, CancellationToken ct = default);
        Task<Result<HealthRecordViewModel?>> GetHealthRecordByIdAsync(int id, CancellationToken ct = default);
        Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);
        Task<Result<UpdateMemberViewModel?>> GetMemberEditedByIdAsync(int id, CancellationToken ct = default);
        Task<Result> UpdateMemberAsync(int id, UpdateMemberViewModel model, CancellationToken ct = default);
        Task<Result> DeleteMemberAsync(int id, CancellationToken ct = default);
    }
}
