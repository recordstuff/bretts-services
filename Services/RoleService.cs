using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;

namespace bretts_services.Services;

public class RoleService : IRoleService
{
    private const string RoleNameIndexName = "IX_Roles_Name";

    private readonly BrettsAppContext _brettsAppContext;
    private readonly IMapper _mapper;

    public RoleService(BrettsAppContext brettsAppContext, IMapper mapper)
    {
        _brettsAppContext = brettsAppContext;
        _mapper = mapper;
    }

    public async Task<List<NameGuidPair>> GetRoles()
    {
        var roles = await _brettsAppContext.Roles
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .ToListAsync();

        return _mapper.Map<List<NameGuidPair>>(roles);
    }

    public async Task<PaginationResult<NameGuidPair>> GetRoles(int page, int pageSize, string? searchText,
        RolesSortColumn sortColumn = RolesSortColumn.Name, SortDirection sortDirection = SortDirection.Ascending)
    {
        IQueryable<Role> query = _brettsAppContext.Roles
            .AsNoTracking();

        if (searchText != null)
        {
            searchText = searchText.ToLower();

            query = query.Where(role => role.Name.ToLower().Contains(searchText));
        }

        var count = await query.CountAsync();

        if (sortDirection == SortDirection.Descending)
        {
            query = sortColumn switch
            {
                RolesSortColumn.Id => query.OrderByDescending(role => role.RoleGuid),
                RolesSortColumn.Name => query.OrderByDescending(role => role.Name),
                _ => query,
            };
        }
        else
        {
            query = sortColumn switch
            {
                RolesSortColumn.Id => query.OrderBy(role => role.RoleGuid),
                RolesSortColumn.Name => query.OrderBy(role => role.Name),
                _ => query,
            };
        }

        var roles = await query.Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResult<NameGuidPair>
        {
            Page = page,
            PageCount = (int)Math.Ceiling((double)count / pageSize),
            ItemCount = count,
            Items = _mapper.Map<List<NameGuidPair>>(roles),
        };
    }

    public async Task<NameGuidPair?> GetRole(Guid guid)
    {
        var role = await _brettsAppContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RoleGuid == guid);

        return _mapper.Map<NameGuidPair>(role);
    }

    public async Task<RoleChangeResult> InsertRole(RoleNew role)
    {
        role.Name = role.Name.Trim();

        var duplicateNameExists = await _brettsAppContext.Roles
            .AsNoTracking()
            .AnyAsync(existingRole => existingRole.Name.ToLower() == role.Name.ToLower());

        if (duplicateNameExists)
        {
            return new RoleChangeResult { Status = RoleChangeStatus.DuplicateName };
        }

        var newRole = _mapper.Map<Role>(role);

        await _brettsAppContext.Roles.AddAsync(newRole);

        if (!await TrySaveRoleChanges())
        {
            return new RoleChangeResult { Status = RoleChangeStatus.DuplicateName };
        }

        return new RoleChangeResult
        {
            Status = RoleChangeStatus.Success,
            Role = _mapper.Map<NameGuidPair>(newRole),
        };
    }

    public async Task<RoleChangeResult> UpdateRole(NameGuidPair role)
    {
        if (!Guid.TryParse(role.Guid, out var roleGuid))
        {
            return new RoleChangeResult { Status = RoleChangeStatus.RoleNotFound };
        }

        role.Name = role.Name.Trim();

        var duplicateNameExists = await _brettsAppContext.Roles
            .AsNoTracking()
            .AnyAsync(existingRole => existingRole.RoleGuid != roleGuid
                                   && existingRole.Name.ToLower() == role.Name.ToLower());

        if (duplicateNameExists)
        {
            return new RoleChangeResult { Status = RoleChangeStatus.DuplicateName };
        }

        var dbRole = await _brettsAppContext.Roles
            .FirstOrDefaultAsync(existingRole => existingRole.RoleGuid == roleGuid);

        if (dbRole == null)
        {
            return new RoleChangeResult { Status = RoleChangeStatus.RoleNotFound };
        }

        _mapper.Map(role, dbRole);

        if (!await TrySaveRoleChanges())
        {
            return new RoleChangeResult { Status = RoleChangeStatus.DuplicateName };
        }

        return new RoleChangeResult
        {
            Status = RoleChangeStatus.Success,
            Role = _mapper.Map<NameGuidPair>(dbRole),
        };
    }

    public async Task<RoleChangeResult> DeleteRole(Guid guid)
    {
        await using var transaction = await _brettsAppContext.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        var role = await _brettsAppContext.Roles
            .Include(existingRole => existingRole.Users)
            .FirstOrDefaultAsync(existingRole => existingRole.RoleGuid == guid);

        if (role == null)
        {
            return new RoleChangeResult { Status = RoleChangeStatus.RoleNotFound };
        }

        if (role.Users.Any())
        {
            return new RoleChangeResult { Status = RoleChangeStatus.RoleInUse };
        }

        _brettsAppContext.Roles.Remove(role);
        await _brettsAppContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return new RoleChangeResult { Status = RoleChangeStatus.Success };
    }

    private async Task<bool> TrySaveRoleChanges()
    {
        try
        {
            await _brettsAppContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
            when (SqlExceptionHelper.IsDuplicateKeyForIndex(ex, RoleNameIndexName))
        {
            return false;
        }
    }
}
