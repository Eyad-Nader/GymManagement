using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.pl.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public async Task<IActionResult> Index(CancellationToken ct )
        {
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Create), model);
            }
            var result = await _memberService.CreateMemberAsync(model, ct);
            if (result)          
                TempData["SuccessMessage"] = "Member created successfully.";
            else
                TempData["ErrorMessage"] = "Failed to create member.";
                
            return RedirectToAction(nameof(Index));         
        }

        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberByIdAsync(id, ct);
            if (member == null)
            {
                TempData["ErrorMessage"] = $"No member found With ID: {id}";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var memberHealthRecord = await _memberService.GetHealthRecordByIdAsync(id, ct);
            if (memberHealthRecord == null)
            {
                TempData["ErrorMessage"] = $"No member HealthRecord found With ID: {id}";
                return RedirectToAction(nameof(Index));
            }
            return View(memberHealthRecord);
        }

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var memberEditViewModel = await _memberService.GetMemberEditedByIdAsync(id, ct);
            if (memberEditViewModel == null)
            {
                TempData["ErrorMessage"] = $"No member found With ID: {id}";
                return RedirectToAction(nameof(Index));
            }
            return View(memberEditViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id, UpdateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(EditMember), model);
            }
            var result = await _memberService.UpdateMemberAsync(id, model, ct);
            if(result)
                TempData["SuccessMessage"] = "Member updated successfully.";
            else
                TempData["ErrorMessage"] = "Failed to update member.";

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberByIdAsync(id, ct);
            if (member == null)
            {
                TempData["ErrorMessage"] = $"No member found With ID: {id}";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int id, CancellationToken ct)
        {
            var result = await _memberService.DeleteMemberAsync(id, ct);
            if (result)
                TempData["SuccessMessage"] = "Member deleted successfully.";
            else
                TempData["ErrorMessage"] = "Failed to delete member.";
            return RedirectToAction(nameof(Index));
        }
        
    }
}
