using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagement.Controllers
{
    public class UsersController : Controller
    {
        EmployeeMgtSystemEntities db = new EmployeeMgtSystemEntities();

        //Function to sign user out
        public ActionResult Signout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        //Function to edit employee details as an employee
        [HttpGet]
        public ActionResult EditProfile()
        {
            EditProfileModels model = new EditProfileModels();
            int id = Convert.ToInt32(Session["eid"].ToString());

            var employee = db.Employees.Single(r => r.emp_ID == id);

            if (employee != null)
            {
                model.emp_ID = employee.emp_ID;
                model.first_name = employee.first_name;
                model.last_name = employee.last_name;
                model.address = employee.address;
                model.phone_number = employee.phone_number;
                model.email = employee.email;
                model.status = employee.status;
                model.role_ID = employee.role_ID;

                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult EditProfile(EditProfileModels model)
        {
            Employee employee = new Employee
            {
                emp_ID = model.emp_ID,
                first_name = model.first_name,
                last_name = model.last_name,
                email = model.email,
                address = model.address,
                phone_number = model.phone_number,
                status = model.status,
                password = model.re_password,
                role_ID = model.role_ID
            };

            //Save changes for employee
            db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            if (Session["role"].ToString() == "Manager")
            {
                return RedirectToAction("ManagerDashboard","Manager");
            }
            else if (Session["role"].ToString() != "Manager" && Session["role"].ToString() != "Admin")
            {
                return RedirectToAction("EmployeeDashboard", "Employee");
            }

            return View(model);
        }

    }
}