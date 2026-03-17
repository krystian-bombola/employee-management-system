using System.Collections.Generic;

public class Operation
{
    public int Id { get; set; }
    public string OperationName { get; set; }

    public ICollection<JobTask> JobTasks { get; set; } = new List<JobTask>();
}