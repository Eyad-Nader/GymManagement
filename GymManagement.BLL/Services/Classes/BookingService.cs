
using AutoMapper;
using GymManagement.BLL.Attachment;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;

using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class BookingService : IBookingService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;
        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }
        
        public async Task<Result<IEnumerable<SessionViewModels>?>> GetAllBookingsAsync(CancellationToken ct = default)
        {
            var AllSessions =  await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCatrgoryAsync(ct);
            if (AllSessions == null || !AllSessions.Any()) return Result<IEnumerable<SessionViewModels>>.NotFound("No sessions found");

            var MappedSessions = AllSessions.Select(s => new SessionViewModels()
            {
                Id = s.Id,
                Capacity = s.Capacity,
                Description = s.Description,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                TrainerName = s.Trainer.Name,
                CategoryName = s.Category.CategoryName,
               // AvailableSlots = s.capacity - s.Bookings.Count() ليه معملناش  كده ؟؟
            });
            foreach (var session in MappedSessions)
            {
                // N+1 problem
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            }
            return Result<IEnumerable<SessionViewModels>>.Ok(MappedSessions);
        }

        #region GetMEmbers
        public async Task<Result<IEnumerable<MemberBooking>?>> GetAllBookingForUpcomingSession(int SessionId, CancellationToken ct = default)
        {
            var Bookings = await _unitOfWork.BookingsRepository.GetAllBookingForSession(SessionId,ct);
            if (Bookings == null) return Result<IEnumerable<MemberBooking>?>.Ok(new List<MemberBooking>());

            var MappedBookings = _mapper.Map<IEnumerable<MemberBooking>>(Bookings);
            return Result<IEnumerable<MemberBooking>?>.Ok(MappedBookings);
        }
        
        public async Task<Result<IEnumerable<MemberAttendance>?>> GetAllBookingForOngoingSession(int SessionId, CancellationToken ct = default)
        {
            var Bookings = await _unitOfWork.BookingsRepository.GetAllBookingForSession(SessionId,ct);
            if (Bookings == null) return Result<IEnumerable<MemberAttendance>?>.Ok(new List<MemberAttendance>());

            var MappedBookings = _mapper.Map<IEnumerable<MemberAttendance>>(Bookings);
            return Result<IEnumerable<MemberAttendance>?>.Ok(MappedBookings);
        }

        #endregion
        public async Task<Result> CreateBooking(int SessionId, CreateMemberBooking model, CancellationToken ct = default)
        {
            var Session = await _unitOfWork.SessionRepository.GetByIdAsync(SessionId,ct);
            if (Session == null) return Result.Fail("Session not found");
            if(Session.StartTime < DateTime.Now ) return Result.Fail("Session is already started");
            if(Session.EndTime <= DateTime.Now ) return Result.Fail("Session is already ended");

            var countOfBookedSlots = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(SessionId, ct);
            if(Session.Capacity == countOfBookedSlots) return Result.Fail("Session is full");

            var now = DateTime.Now;

            var MemberShip = await _unitOfWork.MemberShipsRepository.AnyAsync(
                m => m.MemberId == model.Id &&
                     m.CreatedAt <= now &&
                     m.EndDate >= now
            );
            if (!MemberShip) return Result.Fail("Member Doesn't Have Active Membership");

            // انا عارض بس الي مش مشتركين في السيشن ديه بس زياده تأكيد
            var AlreadyBooked = await _unitOfWork.BookingsRepository.AnyAsync(
                b => b.SessionId == SessionId && b.MemberId == model.Id, ct
            );
            if (AlreadyBooked) return Result.Fail("Member Already Booked In This Session");
            
            var Booking = _mapper.Map<Booking>(model);

            _unitOfWork.GetRepository<Booking>().Add(Booking);
            var result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            return result ? Result.Ok() : Result.Fail("Failed to create Booking");
        }

        public async Task<Result<IEnumerable<MembersSelectViewModel>>> GetMembersForDropdownAsync(int SessionId, CancellationToken ct = default)
        {
            var members = await _unitOfWork.BookingsRepository
                .GetMembersNotBookedInSession(SessionId, ct)
                ?? new List<Member>();

            var mappedMembers = _mapper.Map<IEnumerable<MembersSelectViewModel>>(members)
                                ?? new List<MembersSelectViewModel>();


            return Result<IEnumerable<MembersSelectViewModel>>.Ok(mappedMembers);
        }

        public async Task<Result> MarkAttendanceAsync(int SessionId, int MemberId, CancellationToken ct = default)
        {
            var Booking = await _unitOfWork.BookingsRepository.FirstOrDefaultAsync(
                b => b.SessionId == SessionId && b.MemberId == MemberId, ct
            );
            if (Booking == null) return Result.Fail("Booking not found");
            if (Booking.IsAttended) return Result.Fail("Member Already Mark as Attended");
            //if(Booking.Session.EndTime < DateTime.Now ) return Result.Fail("Session is already ended");
            //if(Booking.Session.StartTime > DateTime.Now ) return Result.Fail("Session is not started yet");
            
            Booking.IsAttended = true;
            _unitOfWork.GetRepository<Booking>().Update(Booking);
            var result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            return result ? Result.Ok() : Result.Fail("Failed to mark attendance");
        }
        public async Task<Result> CancelBooking(int SessionId, int MemberId, CancellationToken ct = default)
        {
            var Booking = await _unitOfWork.BookingsRepository.FirstOrDefaultAsync(
                b => b.SessionId == SessionId && b.MemberId == MemberId, ct
            );
            if (Booking == null) return Result.Fail("Booking not found");

            //if(Booking.Session.StartTime < DateTime.Now) return Result.Fail("Cannot cancel booking for session that has already started");
            _unitOfWork.GetRepository<Booking>().Delete(Booking);
            var result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            return result ? Result.Ok() : Result.Fail("Failed to cancel Booking");
        }


    }
}
