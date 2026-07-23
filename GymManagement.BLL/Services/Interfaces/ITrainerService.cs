using GymManagement.BLL.Common;
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
        Task<Result<IEnumerable<TrainerViewModel>>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<Result<TrainerDetailsViewModel>> GetTrainerByIdAsync(int id, CancellationToken ct = default);
        Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);

        Task<Result<UpdatedTrainerViewModel>> GetTrainerForUpdateAsync(int id, CancellationToken ct = default);
        Task<Result> UpdateTrainerAsync(int id, UpdatedTrainerViewModel model, CancellationToken ct = default);

        Task<Result> DeleteTrainerAsync(int id, CancellationToken ct = default);  
    }
}
