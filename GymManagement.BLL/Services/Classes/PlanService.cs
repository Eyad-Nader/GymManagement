using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {

        private readonly IUnitOfWork _unitOfWork;
        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> EditPlanAsync(int id, EditedPlanViewModel editedPlanViewModel, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if(plan == null) return false;
            if (await IsPlanInUseAsync(id, ct)) return false;

            //plan.Name = editedPlanViewModel.Name;
            plan.Description = editedPlanViewModel.Description;
            plan.Duration = editedPlanViewModel.Duration;
            plan.Price = editedPlanViewModel.Price;
            plan.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            return _unitOfWork.SaveChangesAsync(ct).Result > 0;
        }
        public async Task<EditedPlanViewModel?> GetEditedPlanByIdAsync(int id, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return null;

            //BLL
            if (await IsPlanInUseAsync(id, ct)) return null;

            var editedPlanViewModel = new EditedPlanViewModel
            {
                Name = plan.Name,
                Description = plan.Description,
                Duration = plan.Duration,
                Price = plan.Price
            };
            return editedPlanViewModel;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            var PlanViewModels = plans.Select(p => new PlanViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Duration = p.Duration,
                Price = p.Price,
                IsActive = p.IsActive
            }).ToList();
            return PlanViewModels;

        }
        public async Task<PlanViewModel> GetPlanByIdAsync(int id, CancellationToken ct)
        {
            if (id == null) return null;

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);

            if (plan == null) return null;

            var planViewModel = new PlanViewModel
            {
                Name = plan.Name,
                Description = plan.Description,
                Duration = plan.Duration,
                Price = plan.Price,
                IsActive = plan.IsActive
            };
            return planViewModel;

        }


        public async Task<bool> ToggleActivateAsync(int id, CancellationToken ct)
        {
            
            var plan = await  _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return false;

            if (await IsPlanInUseAsync(id, ct)) return false;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Plan>().Update(plan);

            return _unitOfWork.SaveChangesAsync(ct).Result > 0;

        }
        // Healper method to check if a plan is in use by any active membership
        public async Task<bool> IsPlanInUseAsync(int planId, CancellationToken ct)
        {
            return  await _unitOfWork.GetRepository<MemberShip>().AnyAsync(m => m.PlanId == planId && m.EndDate > DateTime.UtcNow, ct);
            
        }
    }
}
