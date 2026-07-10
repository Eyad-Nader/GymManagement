using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct);
        Task<PlanViewModel?> GetPlanByIdAsync(int id, CancellationToken ct);
        Task<EditedPlanViewModel?> GetEditedPlanByIdAsync(int id, CancellationToken ct);
        Task<bool> EditPlanAsync(int id, EditedPlanViewModel editedPlanViewModel, CancellationToken ct);
        Task<bool> ToggleActivateAsync(int id, CancellationToken ct);

    }
}
