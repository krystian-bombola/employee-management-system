using System.Collections.Generic;
using System.Linq;
using employee_management_system.Data;
using employee_management_system.Models;
using Microsoft.EntityFrameworkCore;

namespace employee_management_system.Repositories;

public class UserRepository
{
    private readonly DatabaseContext _db;

    public UserRepository(DatabaseContext db)
    {
        _db = db;
    }

    public User? GetByIdentifier(string identifier)
        => _db.Users.Include(u => u.Position).FirstOrDefault(u => u.Identifier == identifier);

    public List<User> GetAll()
        => _db.Users.Include(u => u.Position).ToList();

    public bool HasWorkLogs(int userId)
        => _db.WorkLogs.Any(wl => wl.UserId == userId);

    public void Add(User user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();
    }

    public void Update(User user)
    {
        _db.Users.Update(user);
        _db.SaveChanges();
    }

    public void Remove(User user)
    {
        _db.Users.Remove(user);
        _db.SaveChanges();
    }
}
