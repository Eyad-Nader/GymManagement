using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberShipViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberShipService : IMemberShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MemberShipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result> CancelMemberShipAsync(int id, CancellationToken ct = default)
        {
            var MemberShip = await _unitOfWork.MemberShipsRepository.GetByIdAsync(id, ct);
            if (MemberShip is null) return Result.Fail("MemberShip Not Found");

            if (!MemberShip.IsActive) return Result.Fail("Can't Cancel Expired Membership");

            MemberShip.EndDate = DateTime.Now;
            _unitOfWork.GetRepository<MemberShip>().Update(MemberShip);
            if (await _unitOfWork.SaveChangesAsync(ct) <= 0) return Result.Fail("Failed to Cancel MemberShip");
            return Result.Ok();
        }

        public async Task<Result> CreateMemberShipAsync(CreateMemberShipViewModel createMemberShip, CancellationToken ct = default)
        {
            var ExistMember = await _unitOfWork.GetRepository<Member>().GetByIdAsync(createMemberShip.MemberId);
            if (ExistMember is null) return Result.Fail("Member Not Found");

            var ExistPlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(createMemberShip.PlanId);
            if (ExistPlan is null) return Result.Fail("Plan Not Found");

            if (!ExistPlan.IsActive) return Result.Fail("Plan Not Active");

            var now = DateTime.Now;

            var exists = await _unitOfWork.MemberShipsRepository.AnyAsync(
                ms => ms.MemberId == createMemberShip.MemberId &&
                      ms.CreatedAt <= now &&
                      ms.EndDate >= now
            );
            if (exists) return Result.Fail("Member Already Have Active Membership");

            var MemberShip = new MemberShip
            {
                MemberId = createMemberShip.MemberId,
                PlanId = createMemberShip.PlanId,
                CreatedAt = now,
                EndDate = now.AddDays(ExistPlan.Duration)
            };
            _unitOfWork.GetRepository<MemberShip>().Add(MemberShip);
            if (await _unitOfWork.SaveChangesAsync(ct) <= 0) return Result.Fail("Failed to Create Membership");
            return Result.Ok();
        }
        public async Task<Result<IEnumerable<MemberSelectViewModel>>> GetMemberForDropdownAsync(CancellationToken ct = default)
        {
           var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct:ct);
           return Result<IEnumerable<MemberSelectViewModel>>.Ok(members.Select(c => new MemberSelectViewModel
           {
               Id = c.Id,
               MemberName = c.Name
           }));
        }
        public async Task<Result<IEnumerable<PlanSelectViewModel>>> GetPlanForDropdownAsync(CancellationToken ct = default)
        {
            var plans =  await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            return Result<IEnumerable<PlanSelectViewModel>>.Ok(plans.Select(t => new PlanSelectViewModel
            {
                Id = t.Id,
                PlanName = t.Name
            }));
        }

        public async Task<Result<IEnumerable<MemberShipViewModel>?>> GetAllMemberShipsAsync(CancellationToken ct = default)
        {
            var memberShips = await _unitOfWork.MemberShipsRepository.GetAllMemberShipsWithMemberAndPlanAsync(ct);
            if (memberShips is null) return Result<IEnumerable<MemberShipViewModel>?>.Fail("No MemberShips Found");
            var memberShipViewModels = _mapper.Map<IEnumerable<MemberShipViewModel>>(memberShips);
            return Result<IEnumerable<MemberShipViewModel>?>.Ok(memberShipViewModels);
        }
    }
}
