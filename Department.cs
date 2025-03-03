
namespace PerformanceApp.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }

        // Navigation property
        public ICollection<Employee> Employees { get; set; }
    }

}
