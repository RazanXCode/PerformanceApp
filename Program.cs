﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Make sure to use SqlClient or Microsoft.Data.SqlClient based on your version
using Dapper;
using System.Linq;
using PerformanceApp.Models;
using System.Diagnostics; 

// Employee and Project model definitions
// Employee and Project model definitions
// public class Employee
// {
//     public int EmployeeId { get; set; }
//     public string EmployeeName { get; set; }
//     public decimal Salary { get; set; }
//     public int PerformanceRating { get; set; }
//     public int DepartmentId { get; set; }

//     // Navigation property for Department (Many-to-One)
//     public Department Department { get; set; }

//     // Many-to-Many relationship with Project
//     public List<Project> Projects { get; set; } = new List<Project>();
//     public ICollection<EmployeeProject> EmployeeProjects { get; set; }

//     // Bonus property to store the calculated bonus
//     public decimal Bonus { get; set; }
// }



// public class Project
// {
//     public string ProjectName { get; set; }
//     public DateTime ProjectDeadline { get; set; }
// }

class Program
{
    private static string connectionString = "Server=.;Database=SchoolDB;Trusted_Connection=True;TrustServerCertificate=True";

    static void Main(string[] args)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Task 2
            // Fetching employee details along with their assigned projects from the database
            var sql1 = @"
                SELECT 
                    e.EmployeeName AS EmployeeName, 
                    p.ProjectName AS ProjectName, 
                    p.ProjectDeadline AS ProjectDeadline
                FROM Employees e
                JOIN EmployeeProjects ep ON e.EmployeeId = ep.EmployeeId
                JOIN Projects p ON ep.ProjectId = p.ProjectId";

            // Fetch the data using Dapper and map the results to Employee and Project objects
            var employeeDictionary = new Dictionary<string, Employee>();

            var employees = connection.Query<Employee, Project, Employee>(
                sql1,
                (employee, project) =>
                {
                    // Check if the employee already exists in the dictionary
                    if (!employeeDictionary.TryGetValue(employee.EmployeeName, out var employeeEntry))
                    {
                        // If not, add the employee and initialize their project list
                        employeeEntry = employee;
                        employeeDictionary.Add(employee.EmployeeName, employeeEntry);
                    }

                    // Add the project to the employee's project list
                    employeeEntry.Projects.Add(project);

                    return employeeEntry;
                },
                splitOn: "ProjectName" // This tells Dapper to split the results at ProjectName
            );

            // Display the results
            foreach (var employee in employeeDictionary.Values)
            {
                Console.WriteLine($"Employee: {employee.EmployeeName}");
                foreach (var project in employee.Projects)
                {
                    Console.WriteLine($"- Project: {project.ProjectName}, Deadline: {project.ProjectDeadline.ToShortDateString()}");
                }
            }




            // Task3 
            // SQL query to retrieve employees and their assigned projects
            var sql2 = @"
                SELECT 
                    e.EmployeeName AS EmployeeName, 
                    p.ProjectName AS ProjectName, 
                    p.ProjectDeadline AS ProjectDeadline
                FROM Employees e
                JOIN EmployeeProjects ep ON e.EmployeeId = ep.EmployeeId
                JOIN Projects p ON ep.ProjectId = p.ProjectId";

            // Fetch the data using Dapper and map the results to Employee and Project objects
            var employeeDictionary2 = new Dictionary<string, Employee>();

            var employees2 = connection.Query<Employee, Project, Employee>(
                sql2,
                (employee, project) =>
                {
                    // Check if the employee already exists in the dictionary
                    if (!employeeDictionary.TryGetValue(employee.EmployeeName, out var employeeEntry))
                    {
                        // If not, add the employee and initialize their project list
                        employeeEntry = employee;
                        employeeDictionary2.Add(employee.EmployeeName, employeeEntry);
                    }

                    // Add the project to the employee's project list
                    employeeEntry.Projects.Add(project);

                    return employeeEntry;
                },
                splitOn: "ProjectName" // This tells Dapper to split the results at ProjectName
            );

            // Display the results
            foreach (var employee in employeeDictionary.Values)
            {
                Console.WriteLine($"Employee: {employee.EmployeeName}");
                foreach (var project in employee.Projects)
                {
                    // This will work as expected
                    Console.WriteLine($"- Project: {project.ProjectName}, Deadline: {project.ProjectDeadline.ToShortDateString()}");
                }
            }



            // Task 4
            // SQL query to calculate bonuses based on performance ratings and salary
            var bonusSql = @"
                SELECT 
                    e.EmployeeId, 
                    e.EmployeeName, 
                    e.Salary, 
                    e.PerformanceRating, 
                    -- Bonus calculation logic based on performance rating and salary
                    CASE
                        WHEN e.PerformanceRating = 5 THEN e.Salary * 0.20
                        WHEN e.PerformanceRating = 4 THEN e.Salary * 0.15
                        WHEN e.PerformanceRating = 3 THEN e.Salary * 0.10
                        WHEN e.PerformanceRating = 2 THEN e.Salary * 0.05
                        ELSE 0
                    END AS Bonus
                FROM Employees e";

            // Execute the query using Dapper and map the results to Employee objects
            var employeesWithBonuses = connection.Query<Employee>(bonusSql).ToList();

            //Display bonus details
            foreach (var employee in employeesWithBonuses)
            {
                Console.WriteLine($"Employee: {employee.EmployeeName}");
                Console.WriteLine($"Salary: {employee.Salary:C}, Performance Rating: {employee.PerformanceRating}, Bonus: {employee.Bonus:C}");
                Console.WriteLine();
            }

            //Task 5
            // Set department ID for which we want to fetch the financial report
        int departmentId = 1;

        // Task 5 - Compare performance of EF Core and Dapper

        // Measure time taken by EF Core
        var stopwatch = Stopwatch.StartNew();
        Console.WriteLine("Fetching financial report using EF Core:");
        var efCoreReport = new FinancialReportEFCore(new AppDbContext());
        efCoreReport.FetchFinancialReport(departmentId);
        stopwatch.Stop();
        Console.WriteLine($"EF Core Execution Time: {stopwatch.ElapsedMilliseconds} ms\n");

        // Measure time taken by Dapper
        stopwatch.Restart();
        Console.WriteLine("Fetching financial report using Dapper:");
        var dapperReport = new FinancialReportDapper(connectionString);
        dapperReport.FetchFinancialReport(departmentId);
        stopwatch.Stop();
        Console.WriteLine($"Dapper Execution Time: {stopwatch.ElapsedMilliseconds} ms");
    }
}

// EF Core version for fetching financial report
public class FinancialReportEFCore
{
    private readonly AppDbContext _context;

    public FinancialReportEFCore(AppDbContext context)
    {
        _context = context;
    }

    public void FetchFinancialReport(int departmentId)
    {
        var departmentSalary = _context.Employees
            .Where(e => e.DepartmentId == departmentId)
            .Sum(e => e.Salary);

        var projectBudget = _context.Projects
            .Where(p => p.DepartmentId == departmentId)
            .Sum(p => p.Budget);

        Console.WriteLine($"Total Salary for Department {departmentId}: {departmentSalary:C}");
        Console.WriteLine($"Total Project Budget for Department {departmentId}: {projectBudget:C}");
    }
}

// Dapper version for fetching financial report
public class FinancialReportDapper
{
    private readonly string _connectionString;

    public FinancialReportDapper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void FetchFinancialReport(int departmentId)
    {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var departmentSalaryQuery = @"
                SELECT SUM(Salary) 
                FROM Employees 
                WHERE DepartmentId = @DepartmentId";

                var projectBudgetQuery = @"
                SELECT SUM(Budget) 
                FROM Projects 
                WHERE DepartmentId = @DepartmentId";

                var departmentSalary = connection.ExecuteScalar<decimal>(departmentSalaryQuery, new { DepartmentId = departmentId });
                var projectBudget = connection.ExecuteScalar<decimal>(projectBudgetQuery, new { DepartmentId = departmentId });

                Console.WriteLine($"Total Salary for Department {departmentId}: {departmentSalary:C}");
                Console.WriteLine($"Total Project Budget for Department {departmentId}: {projectBudget:C}");
            }

        }


    }



}