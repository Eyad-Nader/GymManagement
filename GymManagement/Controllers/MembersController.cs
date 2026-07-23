using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.Attachment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GymManagement.pl.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;
        public MembersController(IMemberService memberService , IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }

        public async Task<IActionResult> Index(CancellationToken ct )
        {
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members.Value);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.KeepPhoto = true;
                return View(nameof(Create), model);
            }
            var result = await _memberService.CreateMemberAsync(model, ct);
            if (result.Success)          
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                ViewBag.KeepPhoto = true;
                return View(nameof(Create), model);
            }
        }

        public async Task<IActionResult> GetPhoto(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberByIdAsync(id, ct);
            if (!member.Success || string.IsNullOrWhiteSpace(member.Value.Photo))
                return NotFound();
            
            var file = _attachmentService.GetFile("Members", member.Value.Photo);
            if(file is null) return NotFound();
            
            return File(file.Value.stream, file.Value.ContentType);
        }

        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberByIdAsync(id, ct);
            if (!member.Success || member.Value == null)
            {
                TempData["ErrorMessage"] = member.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(member.Value);
        }
        
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var memberHealthRecord = await _memberService.GetHealthRecordByIdAsync(id, ct);
            if (!memberHealthRecord.Success || memberHealthRecord.Value == null)
            {
                TempData["ErrorMessage"] = memberHealthRecord.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(memberHealthRecord.Value);
        }

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var memberEditViewModel = await _memberService.GetMemberEditedByIdAsync(id, ct);
            if (!memberEditViewModel.Success || memberEditViewModel.Value == null)
            {
                TempData["ErrorMessage"] = memberEditViewModel.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(memberEditViewModel.Value);
        }


        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute]int id, UpdateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(EditMember), model);
            }
            var result = await _memberService.UpdateMemberAsync(id, model, ct);
            if(result.Success)
                TempData["SuccessMessage"] = "Member updated successfully.";
            else
                TempData["ErrorMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberByIdAsync(id, ct);
            if (!member.Success || member.Value == null)
            {
                TempData["ErrorMessage"] = member.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(member.Value);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int id, CancellationToken ct)
        {
            var result = await _memberService.DeleteMemberAsync(id, ct);
            if (result.Success)
                TempData["SuccessMessage"] = "Member deleted successfully.";
            else
                TempData["ErrorMessage"] = "Failed to delete member.";
            return RedirectToAction(nameof(Index));
        }
        
    }
}
