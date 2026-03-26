using System.Collections.Generic;
using System.Linq;
using employee_management_system.Data;
using employee_management_system.Models;

namespace employee_management_system.Repositories;

public class JobRepository
{
    private readonly DatabaseContext _db;

    public JobRepository(DatabaseContext db)
    {
        _db = db;
    }

    public Job? GetByName(string jobName)
        => _db.Jobs.FirstOrDefault(j => j.JobName == jobName);

    public List<Job> GetAll()
        => _db.Jobs.ToList();

    public void Add(Job job)
    {
        _db.Jobs.Add(job);
        _db.SaveChanges();
    }

    public void Remove(Job job)
    {
        var jobTasks = _db.JobTasks.Where(jt => jt.JobId == job.Id).ToList();
        foreach (var jt in jobTasks)
        {
            var workLogs = _db.WorkLogs.Where(wl => wl.JobTaskId == jt.Id).ToList();
            _db.WorkLogs.RemoveRange(workLogs);
        }
        _db.JobTasks.RemoveRange(jobTasks);

        _db.Jobs.Remove(job);
        _db.SaveChanges();
    }
}