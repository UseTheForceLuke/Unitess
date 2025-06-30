using TaskManager.Application.Users.Commands;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstraction;

namespace TaskManager.API.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class UserQueries
{
    [Authorize(Policy = "Admin")]
    [GraphQLName("getUsers")]
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting(typeof(UserDtoSortInputType))]
    public IQueryable<UserDto> GetUsers(
       [Service] IApplicationDbContext context)
    {
        return context.Users
            .AsNoTracking()
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username, // not implementd yet in IdentityServer
                Email = u.Email, // not implementd yet in IdentityServer
                Role = u.Role,
            });
    }

}