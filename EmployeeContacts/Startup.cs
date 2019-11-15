using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EmployeeContacts.Data;
using EmployeeContacts.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeContacts
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<EmployeeContactsContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("EmployeeContactsContext")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void InitializeDatabase(IApplicationBuilder app)
        {
            // Ensure that DB Migrations are run at startup so there
            // doesn't need to be a separate step for this when first 
            // installing and running
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<EmployeeContactsContext>())
                {
                    context.Database.Migrate();
                    EnsureDatabaseIsSeeded(context);
                }
            }
        }

        private static void EnsureDatabaseIsSeeded(EmployeeContactsContext context)
        {
            if (!context.Departments.Any())
            {
                var hr = new Department
                {
                    Id = 1,
                    DepartmentName = "Human Resources"
                };

                var finance = new Department
                {
                    Id = 2,
                    DepartmentName = "Finance"
                };

                context.Departments.Add(hr);
                context.Departments.Add(finance);
                context.SaveChanges();

                var employee1 = new Employee
                {
                    Id = 1,
                    DepartmentId = hr.Id,
                    FirstName = "Mike",
                    LastName = "Jones",
                    Title = "HR Representitive",
                    Email = "mike.jones@abc.com",
                    Phone = "555-123-4567"
                };

                var employee2 = new Employee
                {
                    Id = 2,
                    DepartmentId = hr.Id,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Title = "HR Manager",
                    Email = "jane.smith@abc.com",
                    Phone = "555-123-1111"
                };

                var employee3 = new Employee
                {
                    Id = 3,
                    DepartmentId = finance.Id,
                    FirstName = "Hector",
                    LastName = "Flores",
                    Title = "CPA",
                    Email = "hector.flores@abc.com",
                    Phone = "555-123-2222"
                };

                var employee4 = new Employee
                {
                    Id = 4,
                    DepartmentId = finance.Id,
                    FirstName = "Emily",
                    LastName = "Radnor",
                    Title = "CPA",
                    Email = "emilyr@abc.com",
                    Phone = "555-123-4444"
                };

                var employee5 = new Employee
                {
                    Id = 5,
                    DepartmentId = finance.Id,
                    FirstName = "Sarah",
                    LastName = "Jackson",
                    Title = "CPA Intern",
                    Email = "sjackson@abc.com",
                    Phone = "555-123-5555"
                };

                context.Employees.Add(employee1);
                context.Employees.Add(employee2);
                context.Employees.Add(employee3);
                context.Employees.Add(employee4);
                context.Employees.Add(employee5);
                context.SaveChanges();
            }
        }
    }
}
