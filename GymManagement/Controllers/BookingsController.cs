using GymManagement.BLL.Attachment;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.BLL.ViewModels.MemberShipViewModels;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.pl.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var bookings = await _bookingService.GetAllBookingsAsync(ct);
            return View(bookings.Value);
        }
        
        #region GetMwmbers
        public async Task<IActionResult> GetMembersForUpcomingSession(int SessionId, CancellationToken ct)
        {
            ViewBag.SessionId = SessionId;
            var members = await _bookingService.GetAllBookingForUpcomingSession(SessionId, ct);
            if (!members.Success || members.Value == null)
            {
                TempData["ErrorMessage"] = members.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(members.Value);
        }
        public async Task<IActionResult> GetMembersForOngoingSession(int SessionId, CancellationToken ct)
        {
            ViewBag.SessionId = SessionId;
            var Bookings = await _bookingService.GetAllBookingForOngoingSession(SessionId, ct);
            if (!Bookings.Success || Bookings.Value == null)
            {
                TempData["ErrorMessage"] = Bookings.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(Bookings.Value);
        }

        #endregion
        
        #region Create
        [HttpGet]
        public async Task<IActionResult> Create(int SessionId, CancellationToken ct)
        {
            await PopulateDropdowns(SessionId, ct);
            var model = new CreateMemberBooking { SessionId = SessionId };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(int SessionId, CreateMemberBooking model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(SessionId, ct);
                return View(model);
            }
            var result = await _bookingService.CreateBooking(SessionId, model, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                await PopulateDropdowns(SessionId, ct);
                return View(model);
            }
            TempData["SuccessMessage"] = "Booking created successfully.";
            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { SessionId = SessionId });
        }
        private async Task PopulateDropdowns(int SessionId, CancellationToken ct)
        {
            var result = await _bookingService.GetMembersForDropdownAsync(SessionId, ct);

            var members = result.Value ?? new List<MembersSelectViewModel>();

            ViewBag.Members = new SelectList(members, "Id", "Name");
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> Attendance(int SessionId, int MemberId, CancellationToken ct)
        {
            var result = await _bookingService.MarkAttendanceAsync(SessionId, MemberId, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(GetMembersForOngoingSession), new { SessionId = SessionId });
            }
            TempData["SuccessMessage"] = "Attendance marked successfully.";
            return RedirectToAction(nameof(GetMembersForOngoingSession), new { SessionId = SessionId });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int SessionId, int MemberId, CancellationToken ct)
        {
            var result = await _bookingService.CancelBooking(SessionId, MemberId, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(GetMembersForUpcomingSession), new { SessionId = SessionId });
            }
            TempData["SuccessMessage"] = "Booking cancelled successfully.";
            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { SessionId = SessionId });
        }


    }
}
