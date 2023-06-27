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
    public class HomeController : Controller
    {
        EmployeeMgtSystemEntities db = new EmployeeMgtSystemEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //Function for admin dashboard

        [HttpGet]
        public ActionResult AdminDashboard()
        {
            return View();
        }

        //Function for manager dashboard
        [HttpGet]
        public ActionResult ManagerDashboard()
        {
            return View();
        }

        //Function for employee dashboard
        [HttpGet]
        public ActionResult EmployeeDashboard()
        {
            return View();
        }

        //Function for login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModels model)
        {
            //Compare username and password in database
            var CheckEmployee = db.Employees.Where(x => x.email.Equals(model.username) && x.password.Equals(model.password)).FirstOrDefault();

            //If the user exists, check their role and redirect to respective dashboard
            if(CheckEmployee != null)
            {
                //Retrieve user role
                var EmpDetails = from e in db.Employees
                                 join r in db.ROLEs on e.role_ID equals r.role_ID
                                 where e.email == CheckEmployee.email
                                 select r.role_name;

                //Create session to store email, employee id and role name
                Session["email"] = CheckEmployee.email.ToString();
                Session["eid"] = CheckEmployee.emp_ID.ToString();
                Session["role"] = EmpDetails.FirstOrDefault().ToString();

                if(Session["Role"].ToString() == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Admin");
                }
                else if (Session["Role"].ToString() == "Manager")
                {
                    return RedirectToAction("ManagerDashboard", "Manager");
                }
                else if (Session["Role"].ToString() != "Manager" || Session["Role"].ToString() != "Admin")
                {
                    return RedirectToAction("EmployeeDashboard", "Employee");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return Content("Wrong username or password");
            }
        }
    }
}