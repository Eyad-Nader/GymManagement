using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GymManagement.PL.Controllers
{
    [Authorize]
    public class PlansController : Controller
    {

        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await _planService.GetAllPlansAsync(ct);
            return View(result.Value);
        }
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanByIdAsync(id, ct);
            if (!plan.Success)
            {
                TempData["ErrorMessage"] = plan.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(plan.Value);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _planService.GetEditedPlanByIdAsync(id, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Value);
        }
        public async Task<IActionResult> Edit(int id, EditedPlanViewModel editedPlanViewModel, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(editedPlanViewModel);
            }
            var result = await _planService.EditPlanAsync(id, editedPlanViewModel, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = "Plan updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<ActionResult> IsActivate(int id, CancellationToken ct)
        {
            var result = await _planService.ToggleActivateAsync(id, ct);
            if (!result.Success)
                TempData["ErrorMessage"] = result.Message;
            else
                TempData["SuccessMessage"] = "Plan Status Changed.";
            return RedirectToAction(nameof(Index));
        }

    }
}

