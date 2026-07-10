using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.AnalyticsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IAnalyticsService
    {
        #region My Sul
        //Task<Result<int>> GetAllMembersAsync(CancellationToken ct);
        //Task<Result<int>> GetActiveMembersAsync(CancellationToken ct);
        //Task<Result<int>> GetTotalTrainersAsync(CancellationToken ct);
        //Task<Result<int>> GetUpcomingSessionsAsync(CancellationToken ct);
        //Task<Result<int>> GetOngoingSessionsAsync(CancellationToken ct);
        //Task<Result<int>> GetCompletedSessionsAsync(CancellationToken ct);
        #endregion

        Task<Result<AnalyticsViewModel>?> GetDataAsync(CancellationToken ct);
    }
}
