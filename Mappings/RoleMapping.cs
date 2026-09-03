using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;
using Riok.Mapperly.Abstractions;

namespace bretts_services.Mappings;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RoleMapping
{
    [MapProperty(nameof(Role.RoleGuid), nameof(NameGuidPair.Guid))]
    public partial NameGuidPair ToNameGuidPair(Role role);

    public partial List<NameGuidPair> ToNameGuidPairs(List<Role> roles);

    [MapProperty(nameof(NameGuidPair.Guid), nameof(Role.RoleGuid))]
    [MapperIgnoreTarget(nameof(Role.RoleID))]
    [MapperIgnoreTarget(nameof(Role.Users))]
    public partial Role ToRole(NameGuidPair nameGuidPair);

    [MapProperty(nameof(NameGuidPair.Guid), nameof(Role.RoleGuid))]
    [MapperIgnoreTarget(nameof(Role.RoleID))]
    [MapperIgnoreTarget(nameof(Role.Users))]
    public partial void UpdateRole(NameGuidPair source, Role target);

    [MapperIgnoreTarget(nameof(Role.RoleGuid))]
    [MapperIgnoreTarget(nameof(Role.RoleID))]
    [MapperIgnoreTarget(nameof(Role.Users))]
    public partial Role ToRole(RoleNew roleNew);
}