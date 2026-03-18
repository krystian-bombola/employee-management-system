using System;
using System.Collections.Generic;
using employee_management_system.Models;
using employee_management_system.Repositories;

namespace employee_management_system.Services;

public class JobService
{
    private readonly JobRepository _jobRepository;

    public JobService(JobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public List<Job> GetAll() => _jobRepository.GetAll();

    public void Add(string jobName, string description, string status = "Nowe")
    {
        var job = new Job
        {
            JobName = jobName,
            Description = description,
            CreatedAt = DateTime.Now,
            Status = status
        };
        _jobRepository.Add(job);
    }

    public void Remove(string jobName)
    {
        var job = _jobRepository.GetByName(jobName);
        if (job is not null)
            _jobRepository.Remove(job);
    }
}