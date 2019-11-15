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
        
    }
}