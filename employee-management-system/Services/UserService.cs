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

    public void Add(string firstName, string lastName, string identifier, string password, int? positionId)
    {
        var salt = PasswordService.GenerateSalt();
        var hash = PasswordService.HashPassword(password, salt);

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Identifier = identifier,
            PasswordHash = hash,
            PasswordSalt = salt,
            PositionId = positionId,
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

    public bool TryRemove(string firstName, string lastName, string identifier, out string errorMessage)
    {
        errorMessage = string.Empty;

        var user = _userRepository.GetAll()
            .Find(u => u.FirstName == firstName &&
                       u.LastName == lastName &&
                       u.Identifier == identifier);

        if (user is null)
            return true;

        if (_userRepository.HasWorkLogs(user.Id))
        {
            errorMessage = "Nie można usunąć użytkownika, ponieważ ma zapisany czas pracy w historii.";
            return false;
        }

        _userRepository.Remove(user);
        return true;
    }

    public void Update(int id, string firstName, string lastName, string identifier, string? newPassword, int? positionId)
    {
        var user = _userRepository.GetAll().Find(u => u.Id == id);
        if (user is null) return;

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Identifier = identifier;
        user.PositionId = positionId;

        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            var salt = PasswordService.GenerateSalt();
            user.PasswordHash = PasswordService.HashPassword(newPassword, salt);
            user.PasswordSalt = salt;
        }

        _userRepository.Update(user);
    }
}
