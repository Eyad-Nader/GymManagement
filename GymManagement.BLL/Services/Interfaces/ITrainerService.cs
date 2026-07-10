using GymManagement.BLL.ViewModels.TrainerViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<TrainerDetailsViewModel> GetTrainerByIdAsync(int id, CancellationToken ct = default);
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);

        Task<UpdatedTrainerViewModel?> GetTrainerForUpdateAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateTrainerAsync(int id, UpdatedTrainerViewModel model, CancellationToken ct = default);

        Task<bool> DeleteTrainerAsync(int id, CancellationToken ct = default);  
    }
}
