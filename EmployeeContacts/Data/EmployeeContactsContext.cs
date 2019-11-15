using Microsoft.EntityFrameworkCore;
using EmployeeContacts.Models;

namespace EmployeeContacts.Data
{
    public class EmployeeContactsContext : DbContext
    {
        public EmployeeContactsContext (DbContextOptions<EmployeeContactsContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
    }
}