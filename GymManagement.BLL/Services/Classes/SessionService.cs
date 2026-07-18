using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // CreateToDataBase
        public async Task<Result<IEnumerable<SessionViewModels>?>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var AllSessions =  await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCatrgoryAsync(ct);
            if (AllSessions == null || !AllSessions.Any()) return Result<IEnumerable<SessionViewModels>>.NotFound("No sessions found");

            var MappedSessions = AllSessions.Select(s => new SessionViewModels()
            {
                Id = s.Id,
                Capacity = s.Capacity,
                Description = s.Description,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                TrainerName = s.Trainer.Name,
                CategoryName = s.Category.CategoryName,
               // AvailableSlots = s.capacity - s.Bookings.Count() ليه معملناش  كده ؟؟
            });
            foreach (var session in MappedSessions)
            {
                // N+1 problem
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            }
            return Result<IEnumerable<SessionViewModels>>.Ok(MappedSessions);
        }
        
        #region Create Session
        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            if (model.EndTime < DateTime.Now || model.EndTime < model.StartTime) return Result.Validation("Invalid Time");

            // التأكد من الترينر موجود و الكاتيجوري موجوده 

            //Trainer specialty must match Category 
            var Category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId ,ct );
            if (Category == null) return Result.NotFound("Category Not Found");
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer == null) return Result.NotFound("Trainer Not Found");

            var IsValied = Enum.TryParse<Specialization>(Category.CategoryName, true, out var Categoryspecialization);
            if(!IsValied || trainer.Specialization != Categoryspecialization) return Result.Validation("Trainer specialty does not match Category");
            //Trainer must be free in that time slot — no double-booking
            var hasConflict = await _unitOfWork.GetRepository<Session>()
                                    .AnyAsync(s => s.TrainerId == model.TrainerId 
                                 && s.StartTime < model.EndTime 
                                 && s.EndTime > model.StartTime
                                 ,ct);


            var session  = _mapper.Map<Session>(model);
            session.CreatedAt = DateTime.Now;

            _unitOfWork.SessionRepository.Add(session);
            var result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            return result ? Result.Ok() : Result.Fail("Failed to create session");
        }
        public async Task<Result<IEnumerable<CategorySelectViewModel>>> GetCategoryForDropdownAsync(CancellationToken ct = default)
        {
           var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct:ct);
           return Result<IEnumerable<CategorySelectViewModel>>.Ok(categories.Select(c => new CategorySelectViewModel
           {
               Id = c.Id,
               CategoryName = c.CategoryName
           }));
        }
        public async Task<Result<IEnumerable<TrainerSelectViewModel>>> GetTrainerForDropdownAsync(CancellationToken ct = default)
        {
            var trainers =  await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return Result<IEnumerable<TrainerSelectViewModel>>.Ok(trainers.Select(t => new TrainerSelectViewModel
            {
                Id = t.Id,
                Name = t.Name
            }));
        }
        #endregion

        #region Details Session
        public async Task<Result<SessionViewModels>> GetSessionByIdAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryByIdAsync(id, ct);
            if (session is null) return Result<SessionViewModels>.NotFound("Session not found");

            var mappedSession = _mapper.Map<SessionViewModels>(session);
            mappedSession.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);

            return Result<SessionViewModels>.Ok(mappedSession);
        }
        #endregion

        #region Update Session
        public async Task<Result<UpdateSessionViewModel>> GetSessionForUpdateAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session is null) return Result<UpdateSessionViewModel>.NotFound("Session not found");

            if(session.StartTime < DateTime.Now) return Result<UpdateSessionViewModel>.Fail("Cannot update past sessions");
            var BookingsCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            if (BookingsCount > 0) return Result<UpdateSessionViewModel>.Fail("Cannot update session with booked slots");


            var mappedSession = _mapper.Map<UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(mappedSession);
        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session is null) return Result.Fail("Session not found");

            if (session.StartTime < DateTime.Now) return Result.Fail("Cannot Edit past sessions");
            var BookingsCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            if (BookingsCount > 0) return Result.Fail("Cannot Edit session with booked slots");

            if(model.EndTime < DateTime.Now || model.EndTime < model.StartTime) return Result.Validation("Invalid Time");

            var Category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId, ct);
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer == null) return Result.NotFound("Trainer Not Found");

            var IsValied = Enum.TryParse<Specialization>(Category?.CategoryName, true, out var Categoryspecialization);
            if (!IsValied || trainer.Specialization != Categoryspecialization) return Result.Validation("Trainer specialty does not match Category");


            _mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;
            _unitOfWork.SessionRepository.Update(session);

            var result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            return result ? Result.Ok() : Result.Fail("Failed to update session");
        }
        
        public async Task<Result<IEnumerable<TrainerSelectViewModel>>> GetTrainersSpecificCategoryForDropdownAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryByIdAsync(id, ct);
            if (session is null) return Result<IEnumerable<TrainerSelectViewModel>>.NotFound("Session not found");

            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            var IsValied = Enum.TryParse<Specialization>(session.Category.CategoryName, true, out var Categoryspecialization);
            if (!IsValied ) return Result<IEnumerable<TrainerSelectViewModel>>.Fail("Trainer specialty does not match Category");
            trainers = trainers.Where(t => t.Specialization == Categoryspecialization);

            return Result<IEnumerable<TrainerSelectViewModel>>.Ok(trainers.Select(t => new TrainerSelectViewModel
            {
                Id = t.Id,
                Name = t.Name
            }));
        }


        #endregion
        public async Task<Result> DeleteSessionAsync(int id, CancellationToken ct = default)
        {
            var session = await  _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session is null) return Result.Fail("Session not found");
            if(session.EndTime >= DateTime.Now) return Result.Fail("Cannot delete Session That Has not ended yet");
            var BookingsCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            if (BookingsCount>0) return Result.Fail("Cannot delete Session with booked slots");
            _unitOfWork.SessionRepository.Delete(session);
            var result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            return result ? Result.Ok() : Result.Fail("Failed to delete session");
        }
    }
}
