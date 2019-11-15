using System;
using System.Collections.Generic;
using EmployeeContacts.Models;

namespace EmployeeContacts.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }

        // Navigation property to associated Employees
        public List<Employee> Employees { get; set; }
    }
}