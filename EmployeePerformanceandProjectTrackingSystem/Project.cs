public class Project
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public DateTime Deadline { get; set; }
    public double Budget {get; set;}
    // Many-to-many relationship with Employee
    public List<EmployeeProject> EmployeeProjects { get; set; }
}