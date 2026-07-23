
using AutoMapper;
using GymManagement.BLL.Attachment;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;
        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        #region GetMember/s
        public async Task<Result<IEnumerable<MemberViewModel>?>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            return Result<IEnumerable<MemberViewModel>?>.Ok(_mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members));
        }
        public async Task<Result<MemberViewModel?>> GetMemberByIdAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result<MemberViewModel?>.NotFound("Member not found");

            var detailsMemberViewModel = _mapper.Map<Member, MemberViewModel>(member);
            var ActiveMemberShip = await _unitOfWork.GetRepository<MemberShip>().FirstOrDefaultAsync(m => m.MemberId == member.Id && m.EndDate > DateTime.Now, ct);
            if (ActiveMemberShip != null)
            {
                detailsMemberViewModel.PlanName = ActiveMemberShip.Plan.Name;
                detailsMemberViewModel.MembershipStartDate = ActiveMemberShip.CreatedAt.ToString();
                detailsMemberViewModel.MembershipEndDate = ActiveMemberShip.EndDate.ToString();
            }

            return Result<MemberViewModel?>.Ok(detailsMemberViewModel);
        }

        #endregion

        #region update member
        public async Task<Result<HealthRecordViewModel?>> GetHealthRecordByIdAsync(int id, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(h => h.MemberId == id, ct);
            if (healthRecord == null) return Result<HealthRecordViewModel?>.NotFound("Health Record not found");

            return Result<HealthRecordViewModel?>.Ok(_mapper.Map<HealthRecord, HealthRecordViewModel>(healthRecord));

        }
        public async Task<Result<UpdateMemberViewModel?>> GetMemberEditedByIdAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result<UpdateMemberViewModel?>.NotFound("Member not found");

            // cast for UpdateMemberViewModel
            return Result<UpdateMemberViewModel?>.Ok(_mapper.Map<Member, UpdateMemberViewModel>(member));
        }
        public async Task<Result> UpdateMemberAsync(int id, UpdateMemberViewModel model, CancellationToken ct = default)
        {
            // validation
            var EmailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id, ct);
            var PhoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct);

            if (EmailExists || PhoneExists)
            {
                return Result.Fail("Email or Phone already exists");
            }
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result.NotFound("Member not found");

            _mapper.Map(model, member);
            member.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Member>().Update(member);
            
            if(await _unitOfWork.SaveChangesAsync(ct) > 0) return Result.Ok();
            return Result.Fail("Failed to update member");
        }

        #endregion

        #region create
        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // validation
            var EmailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email, ct);
            var PhoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct);

            if (EmailExists || PhoneExists)
                return Result.Fail("Email or Phone already exists");

            var attachment = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "Members", ct);
            if (string.IsNullOrWhiteSpace(attachment))
                return Result.Fail("Failed to upload photo");

            var member = _mapper.Map<CreateMemberViewModel, Member>(model);
            member.Photo = attachment;
            _unitOfWork.GetRepository<Member>().Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            if (result)
            {
                return Result.Ok();
            }
            _attachmentService.Delete("Members", attachment);
            return Result.Fail("Failed to create member");
        }

        #endregion

        #region delete
        public async Task<Result> DeleteMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result.NotFound("Member not found");

            // buissnes logic                                              الحته ديه غلط عشان ميقدرش يوصل للشيسن لسه هنعدلها 
            var ActiveSession = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == id && b.Session.StartTime > DateTime.Now, ct);

            if (ActiveSession) return Result.Fail("Member has active session");

            var attachment = member.Photo;
            _unitOfWork.GetRepository<Member>().Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            if (result)
            {
                _attachmentService.Delete("Members", attachment);
                return Result.Ok();
            }
            return Result.Fail();
        }

        #endregion


    }
}
