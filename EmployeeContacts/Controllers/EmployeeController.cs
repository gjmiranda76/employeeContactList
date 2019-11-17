using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeContacts.Data;
using EmployeeContacts.Managers;
using EmployeeContacts.Models;

namespace EmployeeContacts.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeContactsContext _context;
        private readonly EmployeeManager _employeeMgr;
        private readonly DepartmentManager _deptMgr;


        public EmployeeController(EmployeeContactsContext context)
        {
            _context = context;
           _employeeMgr = new EmployeeManager(_context);
            _deptMgr = new DepartmentManager(_context);
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var employees = _employeeMgr.GetAllEmployeesForDisplay();
            return View(await employees.ToListAsync());
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_deptMgr.GetAllDepartments(), "Id", "Id");
            return View();
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,FirstName,LastName,Title,Email,Phone,DepartmentId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _employeeMgr.AddEmployee(employee);
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_deptMgr.GetAllDepartments(), "Id", "Id", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employee/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = _employeeMgr.GetEmployeeById((int)id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_deptMgr.GetAllDepartments(), "Id", "Id", employee.DepartmentId);
            return View(employee);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,FirstName,LastName,Title,Email,Phone,DepartmentId")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _employeeMgr.UpdateEmployee(employee);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_deptMgr.GetAllDepartments(), "Id", "Id", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employee/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = _employeeMgr.GetEmployeeById((int)id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _employeeMgr.DeleteEmployeeById((int)id);
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _employeeMgr.GetAllEmployees().Any(e => e.Id == id);
        }
    }
}
