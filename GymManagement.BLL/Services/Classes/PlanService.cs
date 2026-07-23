using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagement.BLL.Common;
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
        private readonly IMapper _mapper;
        public PlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<Result> EditPlanAsync(int id, EditedPlanViewModel editedPlanViewModel, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return Result.Fail("Plan not found");
            if (await IsPlanInUseAsync(id, ct)) return Result.Fail("Plan is in use");

            //plan.Name = editedPlanViewModel.Name;
            _mapper.Map(editedPlanViewModel, plan);
            plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().Update(plan);
            return (await _unitOfWork.SaveChangesAsync(ct)) > 0 ? Result.Ok() : Result.Fail("Failed to save changes");
        }
        public async Task<Result<EditedPlanViewModel?>> GetEditedPlanByIdAsync(int id, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return Result<EditedPlanViewModel?>.Fail("Plan not found");

            //BLL
            if (await IsPlanInUseAsync(id, ct)) return Result<EditedPlanViewModel?>.Fail("Plan is in use");

            var editedPlanViewModel = _mapper.Map<EditedPlanViewModel>(plan);
            return Result<EditedPlanViewModel?>.Ok(editedPlanViewModel);
        }
        public async Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken ct)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            var PlanViewModels = _mapper.Map<IEnumerable<PlanViewModel>>(plans);
            return Result<IEnumerable<PlanViewModel>>.Ok(PlanViewModels);

        }
        public async Task<Result<PlanViewModel>> GetPlanByIdAsync(int id, CancellationToken ct)
        {
            if (id == null) return Result<PlanViewModel>.Fail("Invalid ID");

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);

            if (plan == null) return Result<PlanViewModel>.Fail("Plan not found");

            var planViewModel = _mapper.Map<PlanViewModel>(plan);
            return Result<PlanViewModel>.Ok(planViewModel);

        }


        public async Task<Result> ToggleActivateAsync(int id, CancellationToken ct)
        {

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return Result.Fail("Plan not found");

            if (await IsPlanInUseAsync(id, ct)) return Result.Fail("Plan is in use");

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Plan>().Update(plan);

            return (await _unitOfWork.SaveChangesAsync(ct)) > 0 ? Result.Ok() : Result.Fail("Failed to save changes");

        }
        // Healper method to check if a plan is in use by any active membership
        private async Task<bool> IsPlanInUseAsync(int planId, CancellationToken ct)
        {
            return await _unitOfWork.GetRepository<MemberShip>().AnyAsync(m => m.PlanId == planId && m.EndDate > DateTime.UtcNow, ct);

        }
    }
}
