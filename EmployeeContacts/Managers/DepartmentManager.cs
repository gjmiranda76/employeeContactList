using System;
using System.Linq;
using System.Collections.Generic;
using EmployeeContacts.Data;
using EmployeeContacts.Models;

namespace EmployeeContacts.Managers
{
    public class DepartmentManager
    {
        private EmployeeContactsContext _context;
        public DepartmentManager(EmployeeContactsContext context)
        {
            _context = context;
        }

        public void AddDepartment(string deptName)
        {
            //  Only attempt to add the dept if there is NOT one with the same name already
            if (!_context.Departments.Any(d => d.DepartmentName == deptName) && 
                !String.IsNullOrWhiteSpace(deptName))
            {
                _context.Departments.Add(
                    new Department
                    {
                        DepartmentName = deptName
                    }
                );

                _context.SaveChanges();
            }
        }

        public List<Department> GetAllDepartments()
        {
            return _context.Departments.ToList();
        }

        public Department GetDepartmentById(int deptId)
        {
            return _context.Departments.FirstOrDefault(d => d.Id == deptId);
        }

        public Department GetDepartmentByName(string deptName)
        {
            return _context.Departments.FirstOrDefault(d => d.DepartmentName == deptName);
        }

        public void UpdateDepartmemt(Department dept)
        {
            if (DepartmentExists(dept.Id))
            {
                _context.Update(dept);
                _context.SaveChanges();
            }
        }
 
        public void DeleteDepartmemtById(int deptId)
        {
            var dept = _context.Departments.FirstOrDefault(d => d.Id == deptId);

            if (dept != null)
            {
                _context.Remove(dept);
                _context.SaveChanges();
            }
        }
        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(d => d.Id == id);
        }
    }
}