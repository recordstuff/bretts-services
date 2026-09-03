using bretts_services.Mappings;
using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;

namespace bretts_services.Services;

public class RoleService : ServiceBase, IRoleService
{
    private const string RoleNameIndexName = "IX_Roles_Name";

    private readonly RoleMapping _roleMapping;

    public RoleService(BrettsAppContext brettsAppContext, RoleMapping roleMapping)
        : base(brettsAppContext)
    {
        _roleMapping = roleMapping;
    }

    public async Task<List<NameGuidPair>> GetRoles()
    {
        var roles = await _brettsAppContext.Roles
            .AsNoTracking()
            .OrderBy(role => role.Name)
            .ToListAsync();

        return _roleMapping.ToNameGuidPairs(roles);
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
            Items = _roleMapping.ToNameGuidPairs(roles),
        };
    }

    public async Task<NameGuidPair?> GetRole(Guid guid)
    {
        var role = await _brettsAppContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RoleGuid == guid);

        return role == null
        ? null
        : _roleMapping.ToNameGuidPair(role);
    }

    public async Task<RoleSaveResult> InsertRole(RoleNew role)
    {
        role.Name = role.Name.Trim();

        var duplicateNameExists = await _brettsAppContext.Roles
            .AsNoTracking()
            .AnyAsync(existingRole => existingRole.Name.ToLower() == role.Name.ToLower());

        if (duplicateNameExists)
        {
            return new RoleSaveResult { Status = RoleSaveStatus.DuplicateName };
        }

        var newRole = _roleMapping.ToRole(role);

        await _brettsAppContext.Roles.AddAsync(newRole);

        if (!await TrySaveChanges(RoleNameIndexName))
        {
            return new RoleSaveResult { Status = RoleSaveStatus.DuplicateName };
        }

        return new RoleSaveResult
        {
            Status = RoleSaveStatus.Success,
            Role = _roleMapping.ToNameGuidPair(newRole),
        };
    }

    public async Task<RoleSaveResult> UpdateRole(NameGuidPair role)
    {
        if (!Guid.TryParse(role.Guid, out var roleGuid))
        {
            return new RoleSaveResult { Status = RoleSaveStatus.RoleNotFound };
        }

        role.Name = role.Name.Trim();

        var duplicateNameExists = await _brettsAppContext.Roles
            .AsNoTracking()
            .AnyAsync(existingRole => existingRole.RoleGuid != roleGuid
                                   && existingRole.Name.ToLower() == role.Name.ToLower());

        if (duplicateNameExists)
        {
            return new RoleSaveResult { Status = RoleSaveStatus.DuplicateName };
        }

        var dbRole = await _brettsAppContext.Roles
            .FirstOrDefaultAsync(existingRole => existingRole.RoleGuid == roleGuid);

        if (dbRole == null)
        {
            return new RoleSaveResult { Status = RoleSaveStatus.RoleNotFound };
        }

        _roleMapping.UpdateRole(role, dbRole);

        if (!await TrySaveChanges(RoleNameIndexName))
        {
            return new RoleSaveResult { Status = RoleSaveStatus.DuplicateName };
        }

        return new RoleSaveResult
        {
            Status = RoleSaveStatus.Success,
            Role = _roleMapping.ToNameGuidPair(dbRole),
        };
    }

    public async Task<RoleSaveResult> DeleteRole(Guid guid)
    {
        await using var transaction = await _brettsAppContext.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        var role = await _brettsAppContext.Roles
            .Include(existingRole => existingRole.Users)
            .FirstOrDefaultAsync(existingRole => existingRole.RoleGuid == guid);

        if (role == null)
        {
            return new RoleSaveResult { Status = RoleSaveStatus.RoleNotFound };
        }

        if (role.Users.Any())
        {
            return new RoleSaveResult { Status = RoleSaveStatus.RoleInUse };
        }

        _brettsAppContext.Roles.Remove(role);
        await _brettsAppContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return new RoleSaveResult { Status = RoleSaveStatus.Success };
    }
}
