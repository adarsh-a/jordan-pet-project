using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeMgtCore.Controllers
{
    public class EmployeeController : Controller
    {
        //Database connection
        EmployeeMgtCoreContext db = new EmployeeMgtCoreContext();

        //Employee dashboard
        [Authorize(Roles = "Software Engineer, Senior Software Engineer, Team Leader, QA")]
        [HttpGet]
        public IActionResult EmployeeDashboard()
        {
            return View();
        }
    }
}
