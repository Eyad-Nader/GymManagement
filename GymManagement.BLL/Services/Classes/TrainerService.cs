using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
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
        public TrainerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Update Trainer
        public async Task<bool> UpdateTrainerAsync(int id, UpdatedTrainerViewModel model, CancellationToken ct = default)
        {
            var ExistingEmail = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id != id, ct);
            var ExistingPhone = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != id, ct);

            if (ExistingEmail || ExistingPhone)
                return false;

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null)
                return false;

            //trainer.Name = model.Name;
            //trainer.DateOfBirth = model.DateOfBirth;
            // trainer.Gender = model.Gender;
            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.buildingNumber = model.BuildingNumber;
            trainer.Address.City = model.City;
            trainer.Address.Street = model.Street;
            trainer.Specialization = model.Specialization;
            trainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            return _unitOfWork.SaveChangesAsync(ct).Result > 0;
        }

        public async Task<UpdatedTrainerViewModel?> GetTrainerForUpdateAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null)
                return null;

            return new UpdatedTrainerViewModel
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Gender = trainer.Gender,
                DateOfBirth = trainer.DateOfBirth,
                BuildingNumber = trainer.Address.buildingNumber,
                City = trainer.Address.City,
                Street = trainer.Address.Street,
                Specialization = trainer.Specialization
            };
        }

        #endregion 

        #region Create Trainer
        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var ExistingEmail = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct);
            var ExistingPhone = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct);

            if (ExistingEmail || ExistingPhone)
                return false;

            var trainer = new Trainer
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = new Address
                {
                    buildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street
                },
                // validation for Specialization enum is handled in the controller, so we can safely assign it here ,how ??
                Specialization = model.Specialization
            };
             _unitOfWork.GetRepository<Trainer>().Add(trainer);

            return _unitOfWork.SaveChangesAsync(ct).Result > 0;
        }

        #endregion
        
        #region Get Trainer/s
        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);
            return trainers.Select(t => new TrainerViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Phone = t.Phone,
                Specialization = t.Specialization.ToString(),
            }).ToList();

        }

        public async Task<TrainerDetailsViewModel> GetTrainerByIdAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null)
                return null!;

            return new TrainerDetailsViewModel
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Specialization = trainer.Specialization.ToString(),
                DateOfBirth = trainer.DateOfBirth.ToString("yyyy-MM-dd"),
                Address = $"{trainer.Address.buildingNumber} - {trainer.Address.Street} - {trainer.Address.City}"
            };
        }






        #endregion

        #region Delete Trainer

        public async Task<bool> DeleteTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null) return false;

            // buissnes logic                                              الحته ديه غلط عشان ميقدرش يوصل للشيسن لسه هنعدلها 
            var ActiveSession = await _unitOfWork.GetRepository<Session>().AnyAsync(s => s.TrainerId == id && s.StartTime > DateTime.Now, ct);
            if (ActiveSession) return false;

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            return _unitOfWork.SaveChangesAsync(ct).Result > 0;  
        }
        #endregion 
    }
}
