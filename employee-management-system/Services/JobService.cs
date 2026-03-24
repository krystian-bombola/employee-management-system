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

    public void Add(string jobName, string description, List<int> operationIds, string status = "Nowe")
    {
        var job = new Job
        {
            JobName = jobName,
            Description = description,
            CreatedAt = DateTime.Now,
            Status = status
        };

        _jobRepository.Add(job);

        // after saving job (and getting its Id) create JobTasks and save them
        // repository.Add will save and assign Id; so fetch the saved job
        using var db = new Data.DatabaseContext();
        var saved = System.Linq.Enumerable.FirstOrDefault(db.Jobs, j => j.JobName == jobName && j.CreatedAt == job.CreatedAt);
        if (saved is not null)
        {
            int order = 0;
            foreach (var opId in operationIds)
            {
                var jt = new JobTask
                {
                    JobId = saved.Id,
                    OperationId = opId,
                    Order = order++,
                    OperationStart = DateTime.Now,
                    OperationEnd = DateTime.Now,
                    ExecutionTime = TimeSpan.Zero
                };
                db.JobTasks.Add(jt);
            }
            db.SaveChanges();
        }
    }

    public void Remove(string jobName)
    {
        var job = _jobRepository.GetByName(jobName);
        if (job is not null)
            _jobRepository.Remove(job);
    }
}