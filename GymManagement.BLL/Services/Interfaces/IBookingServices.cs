using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.BLL.Common;
using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.BLL.ViewModels.SessionViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<Result<IEnumerable<SessionViewModels>?>> GetAllBookingsAsync(CancellationToken ct = default);
        Task<Result<IEnumerable<MemberBooking>?>> GetAllBookingForUpcomingSession(int SessionId, CancellationToken ct = default);
        Task<Result<IEnumerable<MemberAttendance>?>> GetAllBookingForOngoingSession(int SessionId, CancellationToken ct = default);

        Task<Result<IEnumerable<MembersSelectViewModel>?>> GetMembersForDropdownAsync(int SessionId, CancellationToken ct = default);

        Task<Result> CreateBooking(int SessionId, CreateMemberBooking model, CancellationToken ct = default);

        Task<Result> MarkAttendanceAsync(int SessionId, int MemberId, CancellationToken ct = default);

        Task<Result> CancelBooking(int SessionId, int MemberId, CancellationToken ct = default);
        
    }
}
