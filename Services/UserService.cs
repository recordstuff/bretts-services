﻿using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;

namespace bretts_services.Services;

public class UserService : IUserService
{
    private readonly BrettsAppContext _brettsAppContext;
    private readonly UserOptions _userOptions;
    private readonly IMapper _mapper;

    public UserService(BrettsAppContext brettsAppContext, IOptions<UserOptions> options, IMapper mapper)
    {
        _brettsAppContext = brettsAppContext;
        _userOptions = options.Value;
        _mapper = mapper;
    }

    public async Task<string> Login(UserCredentials userCredintials)
    {
        userCredintials.Email = userCredintials.Email.ToLower();

        var user = await _brettsAppContext.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == userCredintials.Email);

        if (user is null) return string.Empty;

        if (!Hashing.Verify(userCredintials.Password, user.Password, user.Salt)) return string.Empty;

        var roles = user.Roles.ToList();

        return JwtHelper.GetJwtToken(user.Email, user.DisplayName ?? user.Email, _userOptions.SigningKey, _userOptions.Issuer, _userOptions.Audience, roles);
    }

    public async Task<bool> Add(NewUser newUser)
    {
        newUser.Email = newUser.Email.ToLower();

        var user = await _brettsAppContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == newUser.Email);
        
        if (user is not null)
        {
            return false;
        }

        user = new User();

        user.Email = newUser.Email;
        user.DisplayName = newUser.DisplayName;
        user.Password = Hashing.Hash(newUser.Password, out var salt);

        user.Salt = salt;

        var role = await _brettsAppContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == JwtHelper.RoleName(Roles.User));

        if (role is null)
        {
            throw new KeyNotFoundException("The User Role is missing.");
        }

        user.Roles.Add(role);

        _brettsAppContext.Users.Add(user);

        var written = await _brettsAppContext.SaveChangesAsync();

        return written != 0;
    }

    public async Task<PaginationResult<UserSummary>> GetUsers(int page, int pageSize, string? searchText, Roles roleFilter)
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

    public async Task<UserDetail?> InsertUser(UserDetail user)
    {
        if (user.Guid != Guid.Empty)
        {
            var existingUser = await GetUser(user.Guid);

            if (existingUser != null)
            {
                return null;
            }
        }

        var newUser = _mapper.Map<User>(user);

        await _brettsAppContext.Users.AddAsync(newUser);

        await _brettsAppContext.SaveChangesAsync();

        var addedUser = _mapper.Map<UserDetail>(newUser);

        return addedUser;
    }

    public async Task<UserDetail?> UpdateUser(UserDetail user)
    {
        if (user.Guid == Guid.Empty) return null;

        var dbUser = await _brettsAppContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserGuid == user.Guid);

        if (dbUser == null)
        {
            return null;
        }

        _mapper.Map(user, dbUser);

        var roles = await _brettsAppContext.Roles
            .AsNoTracking()
            .ToListAsync();

        foreach (var dbUserRole in dbUser.Roles) 
        {
            var role = roles.FirstOrDefault(r => r.RoleGuid == dbUserRole.RoleGuid);

            ArgumentNullException.ThrowIfNull(role, nameof(role));

            dbUserRole.RoleID = role.RoleID;
        }

        _brettsAppContext.Entry(dbUser).State = EntityState.Modified;
        _brettsAppContext.Users.Update(dbUser);
        
        await _brettsAppContext.SaveChangesAsync();

        var updatedUser = _mapper.Map<UserDetail>(dbUser);

        return updatedUser;
    }
}
