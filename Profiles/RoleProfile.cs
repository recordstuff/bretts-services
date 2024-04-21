using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;

namespace bretts_services.Profiles;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, NameGuidPair>()
            .ForMember(ngp => ngp.Guid, o => o.MapFrom(r => r.RoleGuid));

        CreateMap<NameGuidPair, Role>()
            .ForMember(r => r.RoleGuid, o => o.MapFrom(ngp => ngp.Guid))
            .ForMember(r => r.RoleID, o => o.Ignore())
            .ForMember(r => r.Users, o => o.Ignore());
    }
}
