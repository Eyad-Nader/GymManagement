using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<Result<SessionViewModels>> GetSessionByIdAsync(int id, CancellationToken ct = default);
        Task<Result<IEnumerable<SessionViewModels>?>> GetAllSessionsAsync(CancellationToken ct = default);
        Task<Result> CreateSessionAsync(CreateSessionViewModel model,CancellationToken ct);
        Task<Result<IEnumerable<TrainerSelectViewModel>>> GetTrainerForDropdownAsync(CancellationToken ct = default);
        Task<Result<IEnumerable<CategorySelectViewModel>>> GetCategoryForDropdownAsync(CancellationToken ct = default);

        Task<Result<IEnumerable<TrainerSelectViewModel>>> GetTrainersSpecificCategoryForDropdownAsync(int id, CancellationToken ct = default);
        Task<Result<UpdateSessionViewModel>> GetSessionForUpdateAsync(int id, CancellationToken ct = default);
        Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);

        Task<Result> DeleteSessionAsync(int id, CancellationToken ct = default);
    }
}
