using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagement.Controllers
{
    public class ManagerController : Controller
    {
        EmployeeMgtSystemEntities db = new EmployeeMgtSystemEntities();

        //Function for manager dashboard
        [HttpGet]
        public ActionResult ManagerDashboard()
        {
            return View();
        }

        //Function to allow manager to view his/her team members
        [HttpGet]
        public ActionResult ManagerViewTeamMembers()
        {
            int session = Convert.ToInt32(Session["eid"]);

            var team_Id = (from t in db.TEAMS1
                           where t.manager_ID == session
                           select t.teams_ID).FirstOrDefault();

            if (team_Id.ToString() != null)
            {
                ViewBag.TeamMessage = "";

                //Retrieve all employees under each manager
                var query = from emp in db.Employees
                            join tm in db.TMEMBERS on emp.emp_ID equals tm.emp_ID
                            join role in db.ROLEs on emp.role_ID equals role.role_ID
                            join team in db.TEAMS1 on tm.teams_ID equals team.teams_ID
                            where team.teams_ID == team_Id
                            select new ManagerViewTeamModels
                            {
                                emp_ID = emp.emp_ID,
                                first_name = emp.first_name,
                                last_name = emp.last_name,
                                role_name = role.role_name
                            };

                return View(query);
            }
            else
            {
                ViewBag.TeamMessage = "You are not assigned to any team yet";
                return View();
            }
        }
    }
}