using GymManagement.BLL.ViewModels.MemberViewModels;
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
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);
        Task<MemberViewModel?> GetMemberByIdAsync(int id, CancellationToken ct = default);
        Task<HealthRecordViewModel?> GetHealthRecordByIdAsync(int id, CancellationToken ct = default);
        Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);
        Task<UpdateMemberViewModel?> GetMemberEditedByIdAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateMemberAsync(int id, UpdateMemberViewModel model, CancellationToken ct = default);
        Task<bool> DeleteMemberAsync(int id, CancellationToken ct = default);
    }
}
