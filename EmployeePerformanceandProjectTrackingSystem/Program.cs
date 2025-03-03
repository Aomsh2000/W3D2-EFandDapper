using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
class Program
{
    static void Main()
    {
         string connectionString = "Server=localhost;Database=EmployeeDB;Trusted_Connection=True;TrustServerCertificate=true;";


       // Create DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        using (var context = new AppDbContext(optionsBuilder.Options))
        {
            Console.WriteLine("Database Connected!");

            // LINQ Query Example: find all employees who have worked on
            //  more than 3 projects in the last 6 months
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var employeesWithMoreThan3Projects = context.EmployeeProjects
                .Where(ep => ep.Project.Deadline >= sixMonthsAgo)  
                .GroupBy(ep => ep.EmployeeId)  
                .Where(g => g.Count() > 3)  
                .Select(g => g.Key)  
                .ToList();

            // Output the result
            var employees = context.Employees
                .Where(e => employeesWithMoreThan3Projects.Contains(e.EmployeeId))
                .ToList();

            foreach (var employee in employees)
            {
                Console.WriteLine($"Employee: {employee.EmployeeName} (ID: {employee.EmployeeId})");
            }

   
        } 
        using (var context = new AppDbContext())
        {
            // SQL query to check if the stored procedure exists and create it if not
            string createProcedureQuery = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CalculateBonuses') AND type in (N'P'))
                BEGIN
                    EXEC('
                        CREATE PROCEDURE CalculateBonuses
                        AS
                        BEGIN
                            SELECT EmployeeId, EmployeeName, Salary, Performance,
                                   (Salary * (Performance * 0.05)) AS Bonus
                            FROM Employees;
                        END;');
                END;";

            // Execute the SQL query to create the stored procedure
            context.Database.ExecuteSqlRaw(createProcedureQuery);
            Console.WriteLine("\nStored procedure 'CalculateBonuses' checked/created successfully.");
          
        }
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Execute the stored procedure to calculate bonuses
            string sql = "EXEC CalculateBonuses"; // Calling the stored procedure directly

            // Use Dapper to execute the query and map the result to the Employee model
            // Use Dapper to execute the query and map the result to a dynamic type (without Bonus in Employee model)
            var employees = connection.Query<dynamic>(sql).ToList();
            // Output the results
            Console.WriteLine("Employee Bonuses:");
            foreach (var employee in employees)
            {
                Console.WriteLine($"Employee: {employee.EmployeeName} (ID: {employee.EmployeeId}) - Bonus: {employee.Bonus:C}");
            }
        }

        using (var context = new AppDbContext())
{
    //EF Query to get total salary per department
    var departmentSalaries = context.Departments
        .Include(d => d.Employees)
        .Select(d => new
        {
            DepartmentName = d.DepartmentName,
            TotalSalary = d.Employees.Sum(e => e.Salary)
        })
        .ToList();

    foreach (var department in departmentSalaries)
    {
        Console.WriteLine($"Department: {department.DepartmentName} - Total Salary: {department.TotalSalary:C}");
    }
}
   //Dapper Query to get total salary per department
using (var connection = new SqlConnection(connectionString))
{
    connection.Open();

    var sql = @"
        SELECT d.DepartmentName, SUM(e.Salary) AS TotalSalary
        FROM Departments d
        JOIN Employees e ON d.DepartmentId = e.DepartmentId
        GROUP BY d.DepartmentName";

    var departmentSalaries = connection.Query(sql)
        .Select(d => new
        {
            DepartmentName = d.DepartmentName,
            TotalSalary = d.TotalSalary
        })
        .ToList();

    foreach (var department in departmentSalaries)
    {
        Console.WriteLine($"Department: {department.DepartmentName} - Total Salary: {department.TotalSalary:C}");
    }
}



        //Dapper
        var employeeProjects = GetEmployeeProjectDetails(connectionString);
            Console.WriteLine(employeeProjects);
            // Display the results
            Console.WriteLine("Employee Name | Project Name | Project Deadline");
            Console.WriteLine("--------------------------------------------");
            foreach (var item in employeeProjects)
            {
                Console.WriteLine($"{item.Employee.EmployeeName} | {item.Project.ProjectName} | {item.Project.Deadline}");
            }

        
    } 
     // Method to retrieve employee and project details BY Dapper
 public static IEnumerable<EmployeeProject> GetEmployeeProjectDetails(string connectionString)
{
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();

        string sql = @"
            SELECT 
                e.EmployeeId, e.EmployeeName,  
                p.ProjectId, p.ProjectName, p.Deadline
            FROM 
                EmployeeProjects ep
            INNER JOIN 
                Employees e ON ep.EmployeeId = e.EmployeeId
            INNER JOIN 
                Projects p ON ep.ProjectId = p.ProjectId";
                 var employeeProjects = connection.Query<EmployeeProject, Employee, Project, EmployeeProject>(
            sql,
            (employeeProject, employee, project) =>
            {
                // Map the Employee and Project objects to the EmployeeProject object
                employeeProject.Employee = employee;
                employeeProject.Project = project;
                return employeeProject;
            },
            splitOn: "EmployeeId,ProjectId" // Split the result on EmployeeId and ProjectId
        ).ToList(); // Return as a list

        return employeeProjects;

        
           

       
    }
}

   

}