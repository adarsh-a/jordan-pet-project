using EmployeeMgtCore.DataDB;
using EmployeeMgtCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;

namespace EmployeeMgtCore.Controllers
{
    public class HomeController : Controller
    {
        EmployeeMgtCoreContext db = new EmployeeMgtCoreContext();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Function to login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                //Retrieve the employee info based on the provided email and password
                var CheckEmployee = db.Employees.Where(x => x.Email.Equals(model.email) && x.Password.Equals(model.password)).FirstOrDefault();

                if (CheckEmployee != null)
                {
                    //Retrieve the role name of the user
                    var rolee = await (from r in db.Roles
                                    join e in db.Employees on r.RoleId equals e.RoleId
                                    where e.Email == model.email
                                    select r.Rolename).FirstOrDefaultAsync();

                    //Create session for userid, email and role name
                    HttpContext.Session.SetString("eid", CheckEmployee.EmpId.ToString());
                    HttpContext.Session.SetString("eusername", CheckEmployee.Email.ToString());
                    HttpContext.Session.SetString("erole", rolee);

                    //Creating claims identity based on user role
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Role, rolee)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    //Verify role name after login and redirect user to respective dashboard
                    if (HttpContext.Session.GetString("erole").ToString() == "Admin")
                    {
                        return RedirectToAction("AdminDashboard", "Admin");
                    }

                    if (HttpContext.Session.GetString("erole").ToString() == "Manager")
                    {
                        return RedirectToAction("ManagerDashboard", "Manager");
                    }

                    if (HttpContext.Session.GetString("erole").ToString() != null && HttpContext.Session.GetString("erole").ToString() != "Manager" && HttpContext.Session.GetString("erole").ToString() != "Admin")
                    {
                        return RedirectToAction("EmployeeDashboard", "Employee");
                    }

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            else
            {
                return View(model);
            }
        }
    }
}