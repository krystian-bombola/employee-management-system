using employee_management_system.Models;
using employee_management_system.Repositories;

namespace employee_management_system.Services;

public class AuthService
{
    private readonly UserRepository _userRepository;

    public AuthService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User? Login(string identifier, string password)
    {
        var user = _userRepository.GetByIdentifier(identifier);
        if (user is null)
            return null;

        if (!PasswordService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            return null;

        return user;
    }
}