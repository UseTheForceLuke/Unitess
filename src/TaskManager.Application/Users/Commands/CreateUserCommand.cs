using MediatR;
using TaskManager.Domain.Users;

namespace TaskManager.Application.Users.Commands;

public record CreateUserCommand(
    string Username,
    string Email,
    UserRole Role = UserRole.User) : IRequest<UserDto>;

//public record UserDto(
//    Guid Id,
//    string Username,
//    string Email,
//    UserRole Role
//);

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }

    public UserDto() { }

    public UserDto(User user)
    {
        Id = user.Id;
        Username = user.Username;
        Email = user.Email;
        Role = user.Role;
    }
}
