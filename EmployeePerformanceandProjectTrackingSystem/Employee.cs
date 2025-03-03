public class Employee
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public decimal Salary { get; set; }
    public int Performance {get; set;}
    // Foreign Key for Department
    public int DepartmentId { get; set; }
    public Department Department { get; set; }

    // Many-to-many relationship with Project
    public List<EmployeeProject> EmployeeProjects { get; set; }
}