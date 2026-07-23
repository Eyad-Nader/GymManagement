using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.AnalyticsViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AnalyticsViewModel>?> GetDataAsync(CancellationToken ct)
        {
            var Members = await _unitOfWork.GetRepository<Member>().CountAsync(ct:ct);
            var trainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct:ct);
            var ActiveMembers = await _unitOfWork.GetRepository<MemberShip>().CountAsync(m => m.EndDate > DateTime.Now);
            var OngoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartTime <= DateTime.Now && s.EndTime >= DateTime.Now, ct);
            var CompletedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.EndTime < DateTime.Now, ct);
            var UpcomingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartTime > DateTime.Now, ct);
            return Result<AnalyticsViewModel>.Ok(new AnalyticsViewModel()
            {
                TotalMembers = Members,
                ActiveMembers = ActiveMembers,
                TotalTrainers = trainers,
                CompletedSessions = CompletedSessions,
                UpcomingSessions = UpcomingSessions,
                OngoingSessions = OngoingSessions
            });

        }



        #region My Sul
        //public async Task<Result<int>> GetActiveMembersAsync(CancellationToken ct)
        //{
        //    var ActiveMembers = await _unitOfWork.GetRepository<MemberShip>().GetAllAsync(ct: ct);
        //    return Result<int>.Ok(ActiveMembers.Where(m => m.IsActive).Count());
        //}
        //
        //public async Task<Result<int>> GetAllMembersAsync(CancellationToken ct)
        //{
        //    var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
        //    return Result<int>.Ok(members.Count());
        //
        //}
        //public async Task<Result<int>> GetTotalTrainersAsync(CancellationToken ct)
        //{
        //    var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
        //    return Result<int>.Ok(trainers.Count());
        //}
        //
        //public async Task<Result<int>> GetCompletedSessionsAsync(CancellationToken ct)
        //{
        //    var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync(ct: ct);
        //
        //    sessions = sessions.Where(s => s.EndTime < DateTime.Now);
        //    return Result<int>.Ok(sessions.Count());
        //}
        //
        //public async Task<Result<int>> GetOngoingSessionsAsync(CancellationToken ct)
        //{
        //    var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync(ct: ct);
        //
        //    sessions = sessions.Where(s => s.StartTime <= DateTime.Now && s.EndTime >= DateTime.Now);
        //    return Result<int>.Ok(sessions.Count());
        //}
        //
        //
        //public async Task<Result<int>> GetUpcomingSessionsAsync(CancellationToken ct)
        //{
        //    var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync(ct: ct);
        //
        //    sessions = sessions.Where(s => s.StartTime > DateTime.Now);
        //    return Result<int>.Ok(sessions.Count());
        //}
        #endregion


    }
}
