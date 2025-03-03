namespace PerformanceApp.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }  // Foreign Key for Department

        // Navigation property for Department (Many-to-One)
        public Department Department { get; set; }

        // Many-to-Many relationship with Project
        public ICollection<EmployeeProject> EmployeeProjects { get; set; }

        // New properties for Salary and PerformanceRating
        public decimal Salary { get; set; }  
        public int PerformanceRating { get; set; } 
    }
}
