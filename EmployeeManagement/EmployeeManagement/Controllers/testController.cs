using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagement.Controllers
{
    public class testController : Controller
    {
        EmployeeMgtSystemEntities db = new EmployeeMgtSystemEntities();

        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        
    
}
}
}