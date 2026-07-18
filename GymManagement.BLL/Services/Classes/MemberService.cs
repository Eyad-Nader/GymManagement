
using AutoMapper;
using GymManagement.BLL.Attachment;
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
        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);
        }
        public async Task<MemberViewModel> GetMemberByIdAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return null;

            var detailsMemberViewModel = _mapper.Map<Member, MemberViewModel>(member);
            var ActiveMemberShip = await _unitOfWork.GetRepository<MemberShip>().FirstOrDefaultAsync(m => m.MemberId == member.Id && m.EndDate > DateTime.Now, ct);
            if (ActiveMemberShip != null)
            {
                detailsMemberViewModel.PlanName = ActiveMemberShip.Plan.Name;
                detailsMemberViewModel.MembershipStartDate = ActiveMemberShip.CreatedAt.ToString();
                detailsMemberViewModel.MembershipEndDate = ActiveMemberShip.EndDate.ToString();
            }

            return detailsMemberViewModel;
        }

        #endregion

        #region update member
        public async Task<HealthRecordViewModel?> GetHealthRecordByIdAsync(int id, CancellationToken ct = default)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(h => h.MemberId == id, ct);
            if (healthRecord == null) return null;

            return _mapper.Map<HealthRecord, HealthRecordViewModel>(healthRecord);

        }
        public async Task<UpdateMemberViewModel?> GetMemberEditedByIdAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            // cast for UpdateMemberViewModel
            return _mapper.Map<Member, UpdateMemberViewModel>(member);
        }
        public async Task<bool> UpdateMemberAsync(int id, UpdateMemberViewModel model, CancellationToken ct = default)
        {
            // validation
            var EmailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id, ct);
            var PhoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct);

            if (EmailExists || PhoneExists)
            {
                return false;
            }
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return false;

            _mapper.Map(model, member);
            member.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Member>().Update(member);
            return _unitOfWork.SaveChangesAsync(ct).Result > 0;
        }

        #endregion

        #region create
        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // validation
            var EmailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email, ct);
            var PhoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct);

            if (EmailExists || PhoneExists)
                return false;

            var attachment = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "Members", ct);
            if (string.IsNullOrWhiteSpace(attachment)) return false;

            var member = _mapper.Map<CreateMemberViewModel, Member>(model);
            member.Photo = attachment;
            _unitOfWork.GetRepository<Member>().Add(member);
            var Result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            if (Result)
            {
                return true;
            }
            _attachmentService.Delete("Members", attachment);
            return false;
        }

        #endregion

        #region delete
        public async Task<bool> DeleteMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return false;

            // buissnes logic                                              الحته ديه غلط عشان ميقدرش يوصل للشيسن لسه هنعدلها 
            var ActiveSession = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == id && b.Session.StartTime > DateTime.Now, ct);

            if (ActiveSession) return false;

            var attachment = member.Photo;
            _unitOfWork.GetRepository<Member>().Delete(member);
            var Result = await _unitOfWork.SaveChangesAsync(ct) > 0;
            if (Result)
            {
                _attachmentService.Delete("Members", attachment);
                return true;
            }
            return false;
        }

        #endregion


    }
}
