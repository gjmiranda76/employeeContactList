using System;
using System.Linq;
using System.Collections.Generic;
using EmployeeContacts.Data;
using EmployeeContacts.Models;

namespace EmployeeContacts.Managers
{
    public class EmployeeManager
    {
        private EmployeeContactsContext _context;
        public EmployeeManager(EmployeeContactsContext context)
        {
            _context = context;
        }
        

        public void AddEmployee(Employee employee)
        {
            //  Only attempt to add the employee if there's is not already an employee
            //  with the specified email in the database
            if (!_context.Employees.Any(e => e.Email == employee.Email) )
            {
                _context.Employees.Add(employee);
                _context.SaveChanges();
            }
        }

        public List<Employee> GetAllEmployees()
        {
            return _context.Employees.ToList();
        }

        public List<Employee> GetAllEmployeesForDepartment(int deptId)
        {
            return _context.Employees.Where(e => e.DepartmentId == deptId).ToList();
        }

        public Employee GetEmployeeById(int employeeId)
        {
            return _context.Employees.FirstOrDefault(e => e.Id == employeeId);
        }

        public Employee GetEmployeeByEmail(string employeeEmail)
        {
            return _context.Employees.FirstOrDefault(e => e.Email == employeeEmail);
        }

        public void UpdateEmployee(Employee employee)
        {
            if (EmployeeExists(employee.Id))
            {
                _context.Update(employee);
                _context.SaveChanges();
            }
        }
 
        public void DeleteEmployeeById(int employeeId)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId);

            if (employee != null)
            {
                _context.Remove(employee);
                _context.SaveChanges();
            }
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}