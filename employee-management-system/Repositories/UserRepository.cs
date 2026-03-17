using System.Collections.Generic;
using System.Linq;
using employee_management_system.Data;
using employee_management_system.Models;

namespace employee_management_system.Repositories;

public class UserRepository
{
    private readonly DatabaseContext _db;

    public UserRepository(DatabaseContext db)
    {
        _db = db;
    }

    public User? GetByIdentifier(string identifier)
        => _db.Users.FirstOrDefault(u => u.Identifier == identifier);

    public List<User> GetAll()
        => _db.Users.ToList();

    public void Add(User user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();
    }

    public void Remove(User user)
    {
        _db.Users.Remove(user);
        _db.SaveChanges();
    }
}