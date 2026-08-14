using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;

namespace bretts_services.Services;

public class UserService : ServiceBase, IUserService
{
    private const string UserEmailIndexName = "IX_Users_Email";

    private readonly UserOptions _userOptions;
    private readonly IMapper _mapper;

    public UserService(BrettsAppContext brettsAppContext, IOptions<UserOptions> options, IMapper mapper)
        : base(brettsAppContext)
    {
        _userOptions = options.Value;
        _mapper = mapper;
    }

    public async Task<LoginSession> Login(UserCredentials userCredintials)
    {
        userCredintials.Email = userCredintials.Email.ToLower();

        var user = await _brettsAppContext.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == userCredintials.Email);

        if (user is null) return new LoginSession();

        if (!Hashing.Verify(userCredintials.Password, user.Password, user.Salt)) return new LoginSession();

        var roles = user.Roles.ToList();

        return JwtHelper.GetJwtToken(user.Email, user.DisplayName ?? user.Email, _userOptions.SigningKey, _userOptions.Issuer, _userOptions.Audience, roles);
    }

    public async Task<PaginationResult<UserSummary>> GetUsers(int page, int pageSize, string? searchText, Roles roleFilter,
        UsersSortColumn sortColumn = UsersSortColumn.DisplayName, SortDirection sortDirection = SortDirection.Ascending)
    {
        IQueryable<User> query = _brettsAppContext.Users
            .AsNoTracking();
        
        if (searchText != null)
        {
            searchText = searchText.ToLower();

            query = query.Where(u => u.Email.ToLower().Contains(searchText)
                                  || (u.DisplayName != null && u.DisplayName.ToLower().Contains(searchText)));
        }

        if (roleFilter != Roles.Any)
        {
            var role = await _brettsAppContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == JwtHelper.RoleName(roleFilter));

            if (role is null)
            {
                throw new InvalidOperationException($"Rolefilter: {roleFilter} was not found.");
            }

            query = query.Where(u => u.Roles.Contains(role));
        }

        var count = await query.CountAsync();

        if (sortDirection == SortDirection.Descending)
        {
            query = sortColumn switch
            {
                UsersSortColumn.Id => query.OrderByDescending(u => u.UserGuid),
                UsersSortColumn.DisplayName => query.OrderByDescending(u => u.DisplayName),
                UsersSortColumn.Email => query.OrderByDescending(u => u.Email),
                _ => query,
            };
        }
        else // sortDirection == SortDirection.Ascending
        {
            query = sortColumn switch
            {
                UsersSortColumn.Id => query.OrderBy(u => u.UserGuid),
                UsersSortColumn.DisplayName => query.OrderBy(u => u.DisplayName),
                UsersSortColumn.Email => query.OrderBy(u => u.Email),
                _ => query,
            };
        }
        
        var items = await query.Skip(pageSize * (page - 1))
                               .Take(pageSize)
                               .ToListAsync();

        var paginationResult = new PaginationResult<UserSummary>
        {
            Page = page,
            PageCount = (int)Math.Ceiling((double)count / pageSize),
            ItemCount = count,
            Items = _mapper.Map<List<UserSummary>>(items),
        };

        return paginationResult;
    }

    public async Task<UserDetail?> GetUser(Guid guid)
    {
        var user = await _brettsAppContext.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.UserGuid == guid);

        return _mapper.Map<UserDetail>(user);
    }

    public async Task<UserSaveResult> InsertUser(UserNew user)
    {
        user.Email = user.Email.Trim().ToLower();

        var existingUser = await _brettsAppContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email 
                                   || u.UserGuid == user.Guid);

        if (existingUser != null)
        {
            return new UserSaveResult { Status = UserSaveStatus.DuplicateEmail };
        }

        var newUser = _mapper.Map<User>(user);

        newUser.Password = Hashing.Hash(user.Password, out var salt);
        newUser.Salt = salt;

        var roleGuids = newUser.Roles.Select(r => r.RoleGuid).ToList();

        var roles = await _brettsAppContext.Roles
            .Where(r => roleGuids.Contains(r.RoleGuid))
            .ToListAsync();

        newUser.Roles = roles;

        await _brettsAppContext.Users.AddAsync(newUser);

        if (!await TrySaveChanges(UserEmailIndexName))
        {
            return new UserSaveResult { Status = UserSaveStatus.DuplicateEmail };
        }

        var addedUser = _mapper.Map<UserDetail>(newUser);

        return new UserSaveResult
        {
            Status = UserSaveStatus.Success,
            User = addedUser,
        };
    }

    public async Task<UserSaveResult> UpdateUser(UserDetail user)
    {
        if (user.Guid == Guid.Empty)
        {
            return new UserSaveResult { Status = UserSaveStatus.UserNotFound };
        }

        user.Email = user.Email.Trim().ToLower();

        var emailIsInUse = await _brettsAppContext.Users
            .AsNoTracking()
            .AnyAsync(existingUser => existingUser.UserGuid != user.Guid
                                   && existingUser.Email.ToLower() == user.Email);

        if (emailIsInUse)
        {
            return new UserSaveResult { Status = UserSaveStatus.DuplicateEmail };
        }

        var dbUser = await _brettsAppContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.UserGuid == user.Guid);

        if (dbUser == null)
        {
            return new UserSaveResult { Status = UserSaveStatus.UserNotFound };
        }

        _mapper.Map(user, dbUser);

        var roleGuids = dbUser.Roles.Select(r => r.RoleGuid).ToList();

        var roles = await _brettsAppContext.Roles
            .Where(r => roleGuids.Contains(r.RoleGuid))
            .ToListAsync();

        dbUser.Roles = roles;
          
        _brettsAppContext.Users.Update(dbUser);

        if (!await TrySaveChanges(UserEmailIndexName))
        {
            return new UserSaveResult { Status = UserSaveStatus.DuplicateEmail };
        }

        var updatedUser = _mapper.Map<UserDetail>(dbUser);

        return new UserSaveResult
        {
            Status = UserSaveStatus.Success,
            User = updatedUser,
        };
    }

    public async Task<bool> DeleteUser(Guid guid)
    {
        var user = await _brettsAppContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.UserGuid == guid);

        if (user is null) return false; 

        _brettsAppContext.Users.Remove(user);

        await _brettsAppContext.SaveChangesAsync();

        return true;
    }
}
