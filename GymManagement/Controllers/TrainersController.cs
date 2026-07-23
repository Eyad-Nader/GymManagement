using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.pl.Controllers
{
    [Authorize]
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;
        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var trainers = await _trainerService.GetAllTrainersAsync(ct);
            return View(trainers.Value);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerByIdAsync(id, ct);
            if (!trainer.Success)
            {
                TempData["ErrorMessage"] = trainer.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(trainer.Value);

        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Create), model);
            }
            var result = await _trainerService.CreateTrainerAsync(model, ct);
            if (result.Success)
                TempData["SuccessMessage"] = "Trainer created successfully.";
            else
                TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerForUpdateAsync(id, ct);
            if (!trainer.Success)
            {
                TempData["ErrorMessage"] = trainer.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(trainer.Value);
        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, UpdatedTrainerViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Edit), model);
            }
            var result = await _trainerService.UpdateTrainerAsync(id, model, ct);
            if (result.Success)
                TempData["SuccessMessage"] = "Trainer updated successfully.";
            else
                TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerByIdAsync(id, ct);
            if (!trainer.Success)
            {
                TempData["ErrorMessage"] = trainer.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(trainer.Value);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
        {
            var result = await _trainerService.DeleteTrainerAsync(id, ct);  
            if (result.Success)
                TempData["SuccessMessage"] = "Trainer deleted successfully.";
            else
                TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
