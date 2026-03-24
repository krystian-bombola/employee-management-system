using System.Collections.Generic;
using employee_management_system.Models;
using employee_management_system.Repositories;

namespace employee_management_system.Services;

public class PositionService
{
    private readonly PositionRepository _repository;

    public PositionService(PositionRepository repository)
    {
        _repository = repository;
    }

    public List<Position> GetAll() => _repository.GetAll();

    public void Add(string positionName, decimal hourlyRate)
        => _repository.Add(new Position { PositionName = positionName, HourlyRate = hourlyRate });

    public void Update(int id, string positionName, decimal hourlyRate)
    {
        var position = _repository.GetAll().Find(p => p.Id == id);
        if (position is null) return;

        position.PositionName = positionName;
        position.HourlyRate = hourlyRate;
        _repository.Update(position);
    }

    public void Remove(int id) => _repository.Remove(id);
}