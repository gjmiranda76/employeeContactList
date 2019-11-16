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
