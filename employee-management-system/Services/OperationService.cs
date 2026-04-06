using System.Collections.Generic;
using employee_management_system.Models;
using employee_management_system.Repositories;

namespace employee_management_system.Services;

public class OperationService
{
    private readonly OperationRepository _operationRepository;

    public OperationService(OperationRepository operationRepository)
    {
        _operationRepository = operationRepository;
    }

    public List<Operation> GetAll() => _operationRepository.GetAll();
    public List<string> GetAllNames() => _operationRepository.GetAllNames();

    public void Add(string operationName, string description)
    {
        var operation = new Operation 
        { 
            OperationName = operationName,
            Description = description
        };
        _operationRepository.Add(operation);
    }

    public void Update(int operationId, string operationName, string description)
    {
        var operation = _operationRepository.GetById(operationId);
        if (operation is null)
            return;

        operation.OperationName = operationName;
        operation.Description = description;
        _operationRepository.Update(operation);
    }

    public void Remove(string operationName)
    {
        var operation = _operationRepository.GetByName(operationName);
        if (operation is not null)
            _operationRepository.Remove(operation);
    }
}
