using System.Collections.Generic;
using employee_management_system.Models;
using employee_management_system.Repositories;

namespace employee_management_system.Services;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public List<User> GetAll() => _userRepository.GetAll();

    public void Add(string firstName, string lastName, string identifier)
    {
        var defaultPassword = $"{firstName}123";
        var salt = PasswordService.GenerateSalt();
        var hash = PasswordService.HashPassword(defaultPassword, salt);

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Identifier = identifier,
            PasswordHash = hash,
            PasswordSalt = salt,
            EmploymentDate = System.DateTime.Now.ToString("yyyy-MM-dd")
        };
        _userRepository.Add(user);
    }

    public void Remove(string firstName, string lastName, string identifier)
    {
        var user = _userRepository.GetAll()
            .Find(u => u.FirstName == firstName &&
                       u.LastName == lastName &&
                       u.Identifier == identifier);

        if (user is not null)
            _userRepository.Remove(user);
    }
}