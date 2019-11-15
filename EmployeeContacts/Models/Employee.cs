using EmployeeContacts.Models;

namespace EmployeeContacts.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Foreign key to associated Department
        public int DepartmentId { get; set; }

        // Navigation property to associated Department
        public Department Department { get; set; }
    }
}