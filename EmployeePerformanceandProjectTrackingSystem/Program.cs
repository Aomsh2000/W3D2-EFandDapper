using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
class Program
{
    static void Main()
    {
         string connectionString = "Server=localhost;Database=EmployeeDB;Trusted_Connection=True;";


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
                            SELECT Id, Name, Salary, PerformanceRating,
                                   (Salary * (PerformanceRating * 0.05)) AS Bonus
                            FROM Employees;
                        END;');
                END;";

            // Execute the SQL query to create the stored procedure
            context.Database.ExecuteSqlRaw(createProcedureQuery);
            Console.WriteLine("\nStored procedure 'CalculateBonuses' checked/created successfully.");
        }
    }      

}