using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;
using Riok.Mapperly.Abstractions;

namespace bretts_services.Mappings;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserMapping
{
    [UseMapper]
    private readonly RoleMapping _roleMapping;

    public UserMapping(RoleMapping roleMapping)
    {
        _roleMapping = roleMapping;
    }

    [MapProperty(nameof(User.UserGuid), nameof(UserSummary.Guid))]
    public partial UserSummary ToUserSummary(User user);

    public partial List<UserSummary> ToUserSummaries(List<User> users);

    [MapProperty(nameof(User.UserGuid), nameof(UserDetail.Guid))]
    public partial UserDetail ToUserDetail(User user);

    [MapProperty(nameof(UserDetail.Guid), nameof(User.UserGuid))]
    [MapperIgnoreTarget(nameof(User.UserID))]
    [MapperIgnoreTarget(nameof(User.Password))]
    [MapperIgnoreTarget(nameof(User.Salt))]
    [MapperIgnoreTarget(nameof(User.CreatedAt))]
    public partial User ToUser(UserDetail userDetail);

    [MapProperty(nameof(UserDetail.Guid), nameof(User.UserGuid))]
    [MapperIgnoreTarget(nameof(User.UserID))]
    [MapperIgnoreTarget(nameof(User.Password))]
    [MapperIgnoreTarget(nameof(User.Salt))]
    [MapperIgnoreTarget(nameof(User.CreatedAt))]
    public partial void UpdateUser(UserDetail source, User target);
}