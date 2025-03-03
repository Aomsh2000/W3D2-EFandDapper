using Microsoft.EntityFrameworkCore;
public class AppDbContext : DbContext
{
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<EmployeeProject> EmployeeProjects { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer("Server=.;Database=EmployeeDB;Trusted_Connection=True;TrustServerCertificate=True;");
     public AppDbContext()
    {
    }
     
     // Constructor to accept DbContextOptions */
      public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One-to-Many Relationship: Department and Employee
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)           // An Employee has one Department
            .WithMany(d => d.Employees)          // A Department can have many Employees
            .HasForeignKey(e => e.DepartmentId); // Foreign key is DepartmentId in Employee

        // Many-to-Many Relationship: Employee and Project
        modelBuilder.Entity<EmployeeProject>()
            .HasKey(ep => new { ep.EmployeeId, ep.ProjectId }); // Composite key of EmployeeId and ProjectId

        modelBuilder.Entity<EmployeeProject>()
            .HasOne(ep => ep.Employee)             
            .WithMany(e => e.EmployeeProjects)     
            .HasForeignKey(ep => ep.EmployeeId);   

        modelBuilder.Entity<EmployeeProject>()
            .HasOne(ep => ep.Project)            
            .WithMany(p => p.EmployeeProjects)     
            .HasForeignKey(ep => ep.ProjectId);    


      // Seed data for Employees
        modelBuilder.Entity<Employee>().HasData(
            new Employee { EmployeeId = 1, EmployeeName = "John Doe", Salary = 80000m, Performance = 85, DepartmentId = 1 },
            new Employee { EmployeeId = 2, EmployeeName = "Jane Smith", Salary = 75000m, Performance = 90, DepartmentId = 2 },
            new Employee { EmployeeId = 3, EmployeeName = "Bob Johnson", Salary = 65000m, Performance = 78, DepartmentId = 3 },
            new Employee { EmployeeId = 4, EmployeeName = "Alice Williams", Salary = 70000m, Performance = 88, DepartmentId = 1 },
            new Employee { EmployeeId = 5, EmployeeName = "Charlie Brown", Salary = 90000m, Performance = 95, DepartmentId = 2 },
            new Employee { EmployeeId = 6, EmployeeName = "Emily Davis", Salary = 72000m, Performance = 80, DepartmentId = 3 }
        );
        modelBuilder.Entity<Department>().HasData(
            new Department { DepartmentId = 1, DepartmentName = "HR" },
            new Department { DepartmentId = 2, DepartmentName = "IT" },
            new Department { DepartmentId = 3, DepartmentName = "Sales" }
        );
        modelBuilder.Entity<EmployeeProject>().HasData(
            new EmployeeProject { EmployeeId = 1, ProjectId = 1 }, // John Doe assigned to Project 1
            new EmployeeProject { EmployeeId = 1, ProjectId = 2 }, // John Doe assigned to Project 2
            new EmployeeProject { EmployeeId = 2, ProjectId = 2 }, // Jane Smith assigned to Project 2
            new EmployeeProject { EmployeeId = 2, ProjectId = 3 }, // Jane Smith assigned to Project 3
            new EmployeeProject { EmployeeId = 3, ProjectId = 1 }, // Bob Johnson assigned to Project 1
            new EmployeeProject { EmployeeId = 3, ProjectId = 3 }, // Bob Johnson assigned to Project 3
            new EmployeeProject { EmployeeId = 4, ProjectId = 2 }, // Alice Williams assigned to Project 2
            new EmployeeProject { EmployeeId = 5, ProjectId = 3 }, // Charlie Brown assigned to Project 3
            new EmployeeProject { EmployeeId = 6, ProjectId = 1 }  // Emily Davis assigned to Project 1
        );
         // Seed data for Projects
        modelBuilder.Entity<Project>().HasData(
            new Project { ProjectId = 1, ProjectName = "Project Alpha", Deadline = new DateTime(2025, 12, 31), Budget = 500000 },
            new Project { ProjectId = 2, ProjectName = "Project Beta", Deadline = new DateTime(2024, 6, 30), Budget = 300000 },
            new Project { ProjectId = 3, ProjectName = "Project Gamma", Deadline = new DateTime(2025, 9, 15), Budget = 400000 },
            new Project { ProjectId = 4, ProjectName = "Project AI", Deadline = new DateTime(2023, 9, 15), Budget = 350000 },
            new Project { ProjectId = 5, ProjectName = "Project Sigma", Deadline = new DateTime(2022, 9, 15), Budget = 200000 }
        );
    }   

    

}