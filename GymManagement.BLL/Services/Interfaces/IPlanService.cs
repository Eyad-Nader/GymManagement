using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.PlanViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<Result<IEnumerable<PlanViewModel>>> GetAllPlansAsync(CancellationToken ct);
        Task<Result<PlanViewModel?>> GetPlanByIdAsync(int id, CancellationToken ct);
        Task<Result<EditedPlanViewModel?>> GetEditedPlanByIdAsync(int id, CancellationToken ct);
        Task<Result> EditPlanAsync(int id, EditedPlanViewModel editedPlanViewModel, CancellationToken ct);
        Task<Result> ToggleActivateAsync(int id, CancellationToken ct);

    }
}
