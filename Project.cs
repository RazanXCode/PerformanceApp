namespace PerformanceApp.Models
{
    public class Project
    {
        public int ProjectId { get; set; } // Primary Key
        public string ProjectName { get; set; }

         public int DepartmentId { get; set; }

        // Navigation property for the many-to-many relationship with Employee
        public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
    }
}
