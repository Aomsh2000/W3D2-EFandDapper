public class Department
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }

    // One-to-many relationship with Employee
    public List<Employee> Employees { get; set; }
}
