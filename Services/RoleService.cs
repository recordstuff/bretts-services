using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;

namespace bretts_services.Services;

public class RoleService : IRoleService
{
    private readonly BrettsAppContext _brettsAppContext;
    private readonly IMapper _mapper;

    public RoleService(BrettsAppContext brettsAppContext, IMapper mapper)
    {
        _brettsAppContext = brettsAppContext;
        _mapper = mapper;
    }

    async Task<List<NameGuidPair>> IRoleService.GetRoles()
    {
        var roles = await _brettsAppContext.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .ToListAsync();

        var pairs = _mapper.Map<List<NameGuidPair>>(roles);

        return pairs;
    }
}
