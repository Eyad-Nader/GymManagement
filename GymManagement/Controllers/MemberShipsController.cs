using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberShipViewModels;
using GymManagement.DAL.Data.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.pl.Controllers
{
    [Authorize]
    public class MemberShipsController : Controller
    {
        private readonly IMemberShipService _memberShipService;
        public MemberShipsController(IMemberShipService memberShipService)
        {
            _memberShipService = memberShipService;
        }
        public async Task<ActionResult> Index(CancellationToken ct)
        {
            var MemberShips = await _memberShipService.GetAllMemberShipsAsync(ct);
            return View(MemberShips.Value);
        }

        #region Create MemberShip
        [HttpGet]
        public async Task<ActionResult> Create(CancellationToken ct)
        {
            await PopulateDropdowns(ct);
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(CreateMemberShipViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(ct);
                return View(model);
            }
            var result = await _memberShipService.CreateMemberShipAsync(model, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                await PopulateDropdowns(ct);
                return View(model);
            }
            TempData["SuccessMessage"] = "MemberShip created successfully.";
            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateDropdowns(CancellationToken ct)
        {
            ViewBag.Members = new SelectList((await _memberShipService.GetMemberForDropdownAsync(ct)).Value, "Id", "MemberName");
            ViewBag.Plans = new SelectList((await _memberShipService.GetPlanForDropdownAsync(ct)).Value, "Id", "PlanName");
        }
        #endregion

        #region Cancel MemberShip
        public async Task<ActionResult> Cancel(int id, CancellationToken ct)
        {
            var result = await _memberShipService.CancelMemberShipAsync(id, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = "MemberShip cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        
        
    }
}