using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EmployeeContacts.Data;
using EmployeeContacts.Models;
using EmployeeContacts.Managers;

namespace EmployeeContactsTest
{
    public class DbTestsBase : IDisposable
{
    private const string InMemoryConnectionString = "DataSource=:memory:";
    private readonly SqliteConnection _connection;

    protected readonly EmployeeContactsContext DbContext;
    protected DepartmentManager DepartmentManager;
    protected EmployeeManager EmployeeManager;

    protected DbTestsBase()
    {
        _connection = new SqliteConnection(InMemoryConnectionString);
        _connection.Open();
        var options = new DbContextOptionsBuilder<EmployeeContactsContext>()
                .UseSqlite(_connection)
                .Options;
        DbContext = new EmployeeContactsContext(options);
        DbContext.Database.EnsureCreated();

        DepartmentManager = new DepartmentManager(DbContext);
        EmployeeManager = new EmployeeManager(DbContext);
    }

    public void Dispose()
    {
        _connection.Close();
    }
}

    public class DbTests : DbTestsBase
    {
        #region Department Tests
        [Fact]
        public void AddDepartmentTest()
        {
            Assert.True(DbContext.Departments.Count() == 0);

            DepartmentManager.AddDepartment("deptName1");
            DepartmentManager.AddDepartment("deptName2");

            Assert.True(DbContext.Departments.ToList().Count == 2);
            Assert.True(DbContext.Departments.Any(d => d.DepartmentName == "deptName1"));
            Assert.True(DbContext.Departments.Any(d => d.DepartmentName == "deptName2"));
        }
        
        [Fact]
        public void GetDepartmentTest()
        {
            string deptName = "deptName1";

            Assert.True(DbContext.Departments.Count() == 0);
            DepartmentManager.AddDepartment(deptName);
            Assert.True(DbContext.Departments.Count() == 1);

            var dept = DepartmentManager.GetDepartmentByName(deptName);
            Assert.Equal(dept.DepartmentName, deptName);
        }
        
        [Fact]
        public void UpdateDepartmentTest()
        {
            string deptName = "deptName1";
            string deptNameUpdate = "deptNameUpdated";

            Assert.True(DbContext.Departments.Count() == 0);

            DepartmentManager.AddDepartment(deptName);
            Assert.True(DbContext.Departments.Count() == 1);

            var dept = DepartmentManager.GetDepartmentByName(deptName);
            Assert.Equal(dept.DepartmentName, deptName);

            dept.DepartmentName = deptNameUpdate;
            DepartmentManager.UpdateDepartmemt(dept);

            var updated = DepartmentManager.GetDepartmentByName(deptNameUpdate);
            Assert.Equal(updated.DepartmentName, deptNameUpdate);
        }
        
        [Fact]
        public void RemoveDepartmentTest()
        {
            string deptName = "deptName1";

            Assert.True(DbContext.Departments.Count() == 0);
            DepartmentManager.AddDepartment(deptName);
            Assert.True(DbContext.Departments.Count() == 1);
    
            var dept = DepartmentManager.GetDepartmentByName(deptName);
            Assert.Equal(dept.DepartmentName, deptName);

            DepartmentManager.DeleteDepartmemtById(dept.Id);
            Assert.True(DbContext.Departments.Count() == 0);
        }

        #endregion

        #region  Employee Tests
        [Fact]
        public void AddEmployeeTest()
        {
            SetupDepartments();

            //  DepartmentId 1 is valid since two depts were created
            //  and the IDs were auto generated (and incremented)
            //  starting at 1
            var emp1 = new Employee
            {
                FirstName = "Bill",
                LastName = "Smith",
                Email = "bill.smith@abc.com",
                Title = "Manager I",
                Phone = "555-123-4567",
                DepartmentId = 1
            };

            EmployeeManager.AddEmployee(emp1);
            Assert.True(DbContext.Employees.Any(e => e.Email == emp1.Email));
        }

        [Fact]
        public void AddEmployeeBadDeptIdTest()
        {
            SetupDepartments();

            //  Only two departments were created and neither has and 
            //  ID 0f 49, so this should fail due to the foreign key
            //  contraint not being met 
            var emp1 = new Employee
            {
                FirstName = "Bill",
                LastName = "Smith",
                Email = "bill.smith@abc.com",
                Title = "Manager I",
                Phone = "555-123-4567",
                DepartmentId = 49
            };

            Assert.Throws<DbUpdateException>(() => EmployeeManager.AddEmployee(emp1));
        }

        [Fact]
        public void GetAllEmployeesTest()
        {
            SetupEmployees();
            var employees = EmployeeManager.GetAllEmployees();

            Assert.True(employees.Count == 5);
        }

        [Fact]
        public void GetAllEmployeesForDepartmentTest()
        {
            SetupEmployees();

            var dept1Employees = EmployeeManager.GetAllEmployeesForDepartment(1);
            Assert.True(dept1Employees.Count == 2);

            var dept2Employees = EmployeeManager.GetAllEmployeesForDepartment(2);
            Assert.True(dept2Employees.Count == 3);
        }

        [Fact]
        public void GetEmployeeByIdTest()
        {
            SetupEmployees();

            var emp3 = EmployeeManager.GetEmployeeById(3);
            Assert.True(emp3.Email == "hector.flores@abc.com");
        }

        [Fact]
        public void GetEmployeeByEmailTest()
        {
            SetupEmployees();

            var emp3 = EmployeeManager.GetEmployeeByEmail("hector.flores@abc.com");
            Assert.True(emp3.Id == 3);
        }

        [Fact]
        public void UpdateEmployeeTest()
        {
            SetupEmployees();
    
            var jane = EmployeeManager.GetEmployeeByEmail("jane.smith@abc.com");
            Assert.True(jane.Title == "HR Manager");

            jane.Title = "VP of HR";
            EmployeeManager.UpdateEmployee(jane);
           
            var janeVP = EmployeeManager.GetEmployeeByEmail("jane.smith@abc.com");
            Assert.True(janeVP.Title == "VP of HR");
        }

        [Fact]
        public void DeleteEmployeeTest()
        {
            SetupEmployees();

            EmployeeManager.DeleteEmployeeById(3);
            Assert.True(DbContext.Employees.Count() == 4);
            Assert.False(DbContext.Employees.Any(e => e.Email == "hector.flores@abc.com"));

        }
        #endregion

        #region Private Methods
        private void SetupEmployees()
        {
            SetupDepartments();

            Assert.True(DbContext.Employees.Count() == 0);

            var employee1 = new Employee
            {
                Id = 1,
                DepartmentId = 1,
                FirstName = "Mike",
                LastName = "Jones",
                Title = "HR Representitive",
                Email = "mike.jones@abc.com",
                Phone = "555-123-4567"
            };

            var employee2 = new Employee
            {
                Id = 2,
                DepartmentId = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Title = "HR Manager",
                Email = "jane.smith@abc.com",
                Phone = "555-123-1111"
            };

            var employee3 = new Employee
            {
                Id = 3,
                DepartmentId = 2,
                FirstName = "Hector",
                LastName = "Flores",
                Title = "CPA",
                Email = "hector.flores@abc.com",
                Phone = "555-123-2222"
            };

            var employee4 = new Employee
            {
                Id = 4,
                DepartmentId = 2,
                FirstName = "Emily",
                LastName = "Radnor",
                Title = "CPA",
                Email = "emilyr@abc.com",
                Phone = "555-123-4444"
            };

            var employee5 = new Employee
            {
                Id = 5,
                DepartmentId = 2,
                FirstName = "Sarah",
                LastName = "Jackson",
                Title = "CPA Intern",
                Email = "sjackson@abc.com",
                Phone = "555-123-5555"
            };

            EmployeeManager.AddEmployee(employee1);
            EmployeeManager.AddEmployee(employee2);
            EmployeeManager.AddEmployee(employee3);
            EmployeeManager.AddEmployee(employee4);
            EmployeeManager.AddEmployee(employee5);

            Assert.True(DbContext.Employees.Count() == 5);
            Assert.True(DbContext.Employees.Any(e => e.Email == employee1.Email));
            Assert.True(DbContext.Employees.Any(e => e.Email == employee2.Email));
            Assert.True(DbContext.Employees.Any(e => e.Email == employee3.Email));
            Assert.True(DbContext.Employees.Any(e => e.Email == employee4.Email));
            Assert.True(DbContext.Employees.Any(e => e.Email == employee5.Email));

        }
        private void SetupDepartments()
        {
            Assert.True(DbContext.Departments.Count() == 0);
            DepartmentManager.AddDepartment("deptName1");
            DepartmentManager.AddDepartment("deptName2");
            Assert.True(DbContext.Departments.ToList().Count == 2);
            Assert.True(DbContext.Departments.Any(d => d.DepartmentName == "deptName1"));
            Assert.True(DbContext.Departments.Any(d => d.DepartmentName == "deptName2"));
        }
        #endregion
    }
}
