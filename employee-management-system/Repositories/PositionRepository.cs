using System.Collections.Generic;
using System.Linq;
using employee_management_system.Data;
using employee_management_system.Models;

namespace employee_management_system.Repositories;

public class PositionRepository
{
    private readonly DatabaseContext _db;

    public PositionRepository(DatabaseContext db)
    {
        _db = db;
    }

    public List<Position> GetAll() => _db.Positions.ToList();

    public void Add(Position position)
    {
        _db.Positions.Add(position);
        _db.SaveChanges();
    }

    public void Remove(int id)
    {
        var pos = _db.Positions.Find(id);
        if (pos != null)
        {
            _db.Positions.Remove(pos);
            _db.SaveChanges();
        }
    }

    public void Update(Position position)
    {
        _db.Positions.Update(position);
        _db.SaveChanges();
    }
}