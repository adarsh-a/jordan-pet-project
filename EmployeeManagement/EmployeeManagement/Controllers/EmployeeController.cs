using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagement.Controllers
{
    public class EmployeeController : Controller
    {
        EmployeeMgtSystemEntities db = new EmployeeMgtSystemEntities();

        //Function for employee dashboard
        [HttpGet]
        public ActionResult EmployeeDashboard()
        {
            return View();
        }
    }
}