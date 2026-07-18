using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.pl.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;
        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public async Task<ActionResult> Index(CancellationToken ct)
        {
            var Sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(Sessions.Value);
        }

        #region Create Session
        [HttpGet]
        public async Task<ActionResult> Create(CancellationToken ct)
        {
            await PopulateDropdowns(ct);
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(ct);
                return View(model);
            }
            var result = await _sessionService.CreateSessionAsync(model, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                await PopulateDropdowns(ct);
                return View(model);
            }
            TempData["SuccessMessage"] = "Session created successfully.";
            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateDropdowns(CancellationToken ct)
        {
            ViewBag.Trainers = new SelectList((await _sessionService.GetTrainerForDropdownAsync(ct)).Value, "Id", "Name");
            ViewBag.Categories = new SelectList((await _sessionService.GetCategoryForDropdownAsync(ct)).Value, "Id", "CategoryName");
        }
        #endregion

        #region Details Session
        public async Task<ActionResult> Details(int id, CancellationToken ct)
        {
            var sessionResult = await _sessionService.GetSessionByIdAsync(id, ct);
            if (!sessionResult.Success)
            {
                TempData["ErrorMessage"] = sessionResult.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(sessionResult.Value);
        }
        #endregion

        #region Update Session
        [HttpGet]
        public async Task<ActionResult> Edit(int id, CancellationToken ct)
        {
            var sessionResult = await _sessionService.GetSessionForUpdateAsync(id, ct);
            if (!sessionResult.Success)
            {
                TempData["ErrorMessage"] = sessionResult.Message;
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Trainers = new SelectList((await _sessionService.GetTrainersSpecificCategoryForDropdownAsync(id, ct)).Value, "Id", "Name");
            return View(sessionResult.Value);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList((await _sessionService.GetTrainersSpecificCategoryForDropdownAsync(id, ct)).Value, "Id", "Name");
                return View(model);
            }
            var result = await _sessionService.UpdateSessionAsync(id, model, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                ViewBag.Trainers = new SelectList((await _sessionService.GetTrainersSpecificCategoryForDropdownAsync(id, ct)).Value, "Id", "Name");
                return View(model);
            }
            TempData["SuccessMessage"] = "Session updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete Session
        [HttpGet]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            var sessionResult = await _sessionService.GetSessionByIdAsync(id, ct);
            if (!sessionResult.Success)
            {
                TempData["ErrorMessage"] = sessionResult.Message;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _sessionService.DeleteSessionAsync(id, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = "Session deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}