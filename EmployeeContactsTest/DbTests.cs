using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EmployeeContacts.Data;
using EmployeeContacts.Managers;
using EmployeeContacts.Models;

namespace EmployeeContactsTest
{
    public class DbTestsBase : IDisposable
{
    private const string InMemoryConnectionString = "DataSource=:memory:";
    private readonly SqliteConnection _connection;

    protected readonly EmployeeContactsContext DbContext;
    protected DepartmentManager DepartmentManager;

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
    }

    public void Dispose()
    {
        _connection.Close();
    }
}

    public class DbTests : DbTestsBase
    {
        [Fact]
        public void Test1()
        {
            Assert.True(DbContext.Departments.ToList().Count == 0);

            DepartmentManager.AddDepartment("deptName1");
            DepartmentManager.AddDepartment("deptName2");

            Assert.True(DbContext.Departments.ToList().Count == 2);
            Assert.True(DbContext.Departments.Any(d => d.DepartmentName == "deptName1"));
            Assert.True(DbContext.Departments.Any(d => d.DepartmentName == "deptName2"));
        }
        [Fact]
        public void Test2()
        {
            Assert.True(1 == 1);
        }
    }
}
