using AutoMapper;
using User_Database;

namespace User_Command.Organizations_cmd.Update
{
    public class OrganizationsUpdateDTO
    {
        public int Id { get; set; }
        public string? name { get; set; }
    }

    public class UpdateProfile : Profile
    {
        public UpdateProfile()
        {
            _ = CreateMap<OrganizationsUpdateDTO, Organizations>()
                .ForMember(src => src.Orgid, act => act.MapFrom(dest => dest.Id))
                .ForMember(src => src.OrgName, act => act.MapFrom(dest => dest.name));
        }
    }
}