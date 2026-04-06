using System.Collections.Generic;
using System.Linq;
using employee_management_system.Data;
using employee_management_system.Models;

namespace employee_management_system.Repositories;

public class OperationRepository
{
    private readonly DatabaseContext _db;

    public OperationRepository(DatabaseContext db)
    {
        _db = db;
    }

    public Operation? GetByName(string operationName)
        => _db.Operations.FirstOrDefault(o => o.OperationName == operationName);

    public Operation? GetById(int operationId)
        => _db.Operations.FirstOrDefault(o => o.Id == operationId);

    public List<string> GetAllNames()
        => _db.Operations.Select(o => o.OperationName).ToList();

    public List<Operation> GetAll()
        => _db.Operations.ToList();

    public void Add(Operation operation)
    {
        _db.Operations.Add(operation);
        _db.SaveChanges();
    }

    public void Remove(Operation operation)
    {
        _db.Operations.Remove(operation);
        _db.SaveChanges();
    }

    public void Update(Operation operation)
    {
        _db.Operations.Update(operation);
        _db.SaveChanges();
    }
}
