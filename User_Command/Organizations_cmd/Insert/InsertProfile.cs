using AutoMapper;
using User_Database;

namespace User_Command.Organizations_cmd.Insert
{
    public class OrganizationsInsertDTO
    {
        public string? name { get; set; }
    }

    public class InsertProfile : Profile
    {
        public InsertProfile()
        {
            _ = CreateMap<OrganizationsInsertDTO, Organizations>()
                .ForMember(src => src.OrgName, act => act.MapFrom(dest => dest.name));
        }
    }
}