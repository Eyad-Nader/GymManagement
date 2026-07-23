using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
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
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TrainerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Update Trainer
        public async Task<Result> UpdateTrainerAsync(int id, UpdatedTrainerViewModel model, CancellationToken ct = default)
        {
            var ExistingEmail = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id != id, ct);
            var ExistingPhone = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != id, ct);

            if (ExistingEmail || ExistingPhone)
                return Result.Fail("Email or Phone already exists");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null)
                return Result.NotFound($"No Trainer found With ID: {id}");

            //trainer.Name = model.Name;
            //trainer.DateOfBirth = model.DateOfBirth;
            // trainer.Gender = model.Gender;
            var mappedTrainer = _mapper.Map<UpdatedTrainerViewModel, Trainer>(model, trainer);

            mappedTrainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().Update(mappedTrainer);
            return await _unitOfWork.SaveChangesAsync(ct) > 0 ? Result.Ok() : Result.Fail("Failed to update trainer");
        }

        public async Task<Result<UpdatedTrainerViewModel>> GetTrainerForUpdateAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null)
                return Result<UpdatedTrainerViewModel>.NotFound($"No Trainer found With ID: {id}");

            return Result<UpdatedTrainerViewModel>.Ok(_mapper.Map<Trainer, UpdatedTrainerViewModel>(trainer));

        }

        #endregion 

        #region Create Trainer
        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var ExistingEmail = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct);
            var ExistingPhone = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct);

            if (ExistingEmail || ExistingPhone)
                return Result.Fail("Email or Phone already exists");

            var trainer = _mapper.Map<CreateTrainerViewModel, Trainer>(model);
            _unitOfWork.GetRepository<Trainer>().Add(trainer);

            return await _unitOfWork.SaveChangesAsync(ct) > 0 ? Result.Ok() : Result.Fail("Failed to create trainer");
        }

        #endregion

        #region Get Trainer/s
        public async Task<Result<IEnumerable<TrainerViewModel>>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return Result<IEnumerable<TrainerViewModel>>.Ok(_mapper.Map<IEnumerable<Trainer>, IEnumerable<TrainerViewModel>>(trainers));
        }

        public async Task<Result<TrainerDetailsViewModel>> GetTrainerByIdAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null)
                return Result<TrainerDetailsViewModel>.NotFound($"No Trainer found With ID: {id}");

            return Result<TrainerDetailsViewModel>.Ok(_mapper.Map<Trainer, TrainerDetailsViewModel>(trainer));
        }
        #endregion

        #region Delete Trainer

        public async Task<Result> DeleteTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null) return Result.NotFound($"No Trainer found With ID: {id}");

            // buissnes logic                                              الحته ديه غلط عشان ميقدرش يوصل للشيسن لسه هنعدلها 
            var ActiveSession = await _unitOfWork.GetRepository<Session>().AnyAsync(s => s.TrainerId == id && s.StartTime > DateTime.Now, ct);
            if (ActiveSession) return Result.Fail("Trainer has active sessions");

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            if (await _unitOfWork.SaveChangesAsync(ct) > 0)
                return Result.Ok();
            else
                return Result.Fail("Failed to delete trainer");

        }


        #endregion
    }
}
