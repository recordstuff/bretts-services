using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;

namespace bretts_services.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserSummary>()
            .ForMember(us => us.Guid, o => o.MapFrom(u => u.UserGuid));

        CreateMap<User, UserDetail>()
            .ForMember(ud => ud.Guid, o => o.MapFrom(u => u.UserGuid))
            .ForMember(ud => ud.Roles, o => o.MapFrom(u => u.Roles));

        CreateMap<UserDetail, User>()
            .ForMember(u => u.UserID, o => o.Ignore())
            .ForMember(u => u.UserGuid, o => o.MapFrom(ud => ud.Guid))
            .ForMember(u => u.Password, o => o.Ignore())
            .ForMember(u => u.Salt, o => o.Ignore())
            .ForMember(u => u.CreatedAt, o => o.Ignore())
            .ForMember(u => u.Roles, o => o.MapFrom(ud => ud.Roles));
    }
}
