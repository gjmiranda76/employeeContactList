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
    public class DepartmentController : Controller
    {
        private readonly EmployeeContactsContext _context;
        private readonly EmployeeManager _employeeMgr;
        private readonly DepartmentManager _deptMgr;

        public DepartmentController(EmployeeContactsContext context)
        {
            _context = context;
            _employeeMgr = new EmployeeManager(_context);
            _deptMgr = new DepartmentManager(_context);
        }

        // GET: Department
        public IActionResult Index()
        {
            
            return View(_deptMgr.GetAllDepartments());
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,DepartmentName")] Department department)
        {
            if (ModelState.IsValid)
            {
                _deptMgr.AddDepartment(department.DepartmentName);
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Department/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = _deptMgr.GetDepartmentById((int)id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,DepartmentName")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _deptMgr.UpdateDepartmemt(department);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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
            return View(department);
        }

        public IActionResult Contacts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeContactsContext = _employeeMgr.GetAllEmployeesForDepartment((int)id);
            return View(employeeContactsContext);
        }


        // GET: Department/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = _deptMgr.GetDepartmentById((int)id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _deptMgr.DeleteDepartmemtById((int)id);
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _deptMgr.GetAllDepartments().Any(e => e.Id == id);
        }
    }
}
