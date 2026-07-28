using AutoMapper;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.BLL.ViewModels.MemberShipViewModels;
using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GymManagement.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            SessionMapping();
            MemberMapping();
            TrainerMapping();
            PlanMapping();
            MemberShipsMapping();

        }
        private void SessionMapping()
        {

            CreateMap<CreateSessionViewModel, Session>().ReverseMap();
            CreateMap<Session, SessionViewModels>()
                .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));
            CreateMap<UpdateSessionViewModel, Session>().ReverseMap();
        }
        private void MemberMapping()
        {
            #region Member Mapping
            CreateMap<Member, MemberViewModel>()
                //.ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.buildingNumber} - {src.Address.Street} - {src.Address.City}"))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString()));

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<Member, UpdateMemberViewModel>()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.buildingNumber))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City));
            CreateMap<UpdateMemberViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                {
                    buildingNumber = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City
                }));

            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                {
                    buildingNumber = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City
                }))
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel));
            #endregion
        }

        private void TrainerMapping()
        {
            CreateMap<Trainer, TrainerViewModel>()
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization.ToString()));

            CreateMap<Trainer, TrainerDetailsViewModel>()
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization.ToString()))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString()))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.buildingNumber} - {src.Address.Street} - {src.Address.City}"));

            CreateMap<CreateTrainerViewModel, Trainer>()
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
            {
                buildingNumber = src.BuildingNumber,
                Street = src.Street,
                City = src.City
            }));


            CreateMap<Trainer, UpdatedTrainerViewModel>()
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization.ToString()))
            .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.buildingNumber))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City));

            CreateMap<UpdatedTrainerViewModel, Trainer>()
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore())
            .ForMember(dest => dest.Gender, opt => opt.Ignore())
            .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
            {
                buildingNumber = src.BuildingNumber,
                Street = src.Street,
                City = src.City
            }));
        }

        private void PlanMapping()
        {
            CreateMap<Plan, PlanViewModel>().ReverseMap();
            CreateMap<Plan, EditedPlanViewModel>();
            CreateMap<EditedPlanViewModel, Plan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.Name, opt => opt.Ignore());
        }

        private void MemberShipsMapping()
        {
            CreateMap<MemberShip, MemberShipViewModel>()
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.Name))
            .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Plan.Name))
            .ForMember(dest => dest.MemberId, opt => opt.MapFrom(src => src.MemberId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreatedAt));
            
        }
    }
}
