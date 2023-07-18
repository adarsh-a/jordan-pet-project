using EmployeeMgtCore.DataDB;
using EmployeeMgtCore.Models;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Net.Mail;
using System.Text;

namespace EmployeeMgtCore.Controllers
{
    public class ManagerController : Controller
    {
        //Database connection
        EmployeeMgtCoreContext db = new EmployeeMgtCoreContext();

        private readonly IBackgroundJobClient backgroundJobClient;

        private readonly ILogger<HomeController> _logger;
        public ManagerController(ILogger<HomeController> logger, IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            this.backgroundJobClient = backgroundJobClient;
        }

        //Manager dashboard
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult ManagerDashboard()
        {
            return View();
        }

        //Function to view all team members
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public IActionResult MyTeam() 
        {
            //Get the session of the user(manager)
            int session = Convert.ToInt32(HttpContext.Session.GetString("eid"));

            //Retrieve all the employees working with the manager and map it onto the model
            var query = from employee in db.Employees
                        join role in db.Roles on employee.RoleId equals role.RoleId
                        join tmember in db.Tmembers on employee.EmpId equals tmember.EmpId
                        join team in db.Teams on tmember.TeamId equals team.TeamId
                        where team.ManagerId == session
                        select new ViewManagerTeamModel
                        {
                            empID = employee.EmpId,
                            empname = employee.Fname + " " + employee.Lname,
                            email = employee.Email,
                            phone = employee.Phonenum,
                            rolename = role.Rolename
                        };

            return View(query);
        }

        //Function to promote employee
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public IActionResult PromoteEmployee(int id)
        {
            //Get the employee details and map it on the model
            var query = (from employee in db.Employees
                        join role in db.Roles on employee.RoleId equals role.RoleId
                        where employee.EmpId == id

                        select new PromotionModel
                        {
                            empID = employee.EmpId,
                            empname = employee.Fname + " " + employee.Lname,
                            email = employee.Email,
                            phonenum = employee.Phonenum,
                            rolename = role.Rolename
                        }).FirstOrDefault();

            return View(query);
        }

        //Use of hang fire to promote the employee
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> PromoteEmployee(int id, PromotionModel model)
        {
            backgroundJobClient.Schedule(() => AllowPromotion(id, model), TimeSpan.FromMinutes(2));
            return RedirectToAction("ManagerDashboard");
        }

        //Function to send email and record promotion for employee
        [HttpPost]
        [NonAction]
        public async Task<IActionResult> AllowPromotion(int id, PromotionModel model)
        {
            //Store role names in variables
            string m = "Manager";
            string se = "Software Engineer";
            string sse = "Senior Software Engineer";
            string tl = "Team Leader";
            string qa = "QA";

            //Define an empty variable for new role
            string newRole = "";

            //Store date in variable
            DateTime dt = DateTime.Now;
            string date = dt.ToShortDateString();

            //Retrieve the actual role name of the current user
            var actualRole = await (from r in db.Roles
                                    join e in db.Employees on r.RoleId equals e.RoleId
                                    where e.EmpId == id
                                    select r.Rolename).FirstOrDefaultAsync();

            //Retrieve the id of the actual role name of the user
            var actualID = await (from r in db.Roles
                                  where r.Rolename == actualRole
                                  select r.RoleId).FirstOrDefaultAsync();

            //Verify if the user actual role is not empty
            if (actualRole != null)
            {
                //Verify if the role is software engineer and assign senior software engineer as new role
                if (actualRole == se)
                {
                    newRole = sse;
                }

                //Verify if the role is senior software engineer and assign team leader as new role
                if (actualRole == sse)
                {
                    newRole = tl;
                }

                //Verify if the role is team leader and assign manager as new role
                if (actualRole == tl)
                {
                    newRole = m;
                }

                //Verify if the role is qa and assign software engineer as new role
                if (actualRole == qa)
                {
                    newRole = se;
                }

                //Retrieve the new role id from the new role assigned to the employee
                var newRoleID = await (from r in db.Roles
                                       where r.Rolename == newRole
                                       select r.RoleId).FirstOrDefaultAsync();

                //Retrieve all the employee details from the id
                var employee = await db.Employees.FindAsync(id);

                if (newRoleID != null)
                {
                    if (employee != null)
                    {
                        //Verify if the new role assign to the employee is manger
                        if (newRole == m)
                        {
                            //Verify if the employee is in a team and if yes, remove the employee from team members
                            var newManager = await db.Tmembers.Where(e => e.EmpId == employee.EmpId).FirstOrDefaultAsync();

                            if (newManager != null)
                            {
                                employee.Fname = employee.Fname;
                                employee.Lname = employee.Lname;
                                employee.Address = employee.Address;
                                employee.Phonenum = employee.Phonenum;
                                employee.Email = employee.Email;
                                employee.RoleId = newRoleID;

                                string empname = employee.Lname + " " + employee.Fname;

                                Promotion promotion = new Promotion
                                {
                                    EmpId = id,
                                    Oldrole = actualID,
                                    Newrole = newRoleID,
                                    Datecreated = Convert.ToDateTime(date),
                                };

                                //Perform add, remove and send email operation
                                db.Promotions.Add(promotion);
                                db.Tmembers.Remove(newManager);
                                await db.SaveChangesAsync();
                                PromotionEmail(empname, employee.Email, actualRole, newRole);
                                return RedirectToAction("MyTeam");
                            }
                        }
                        else
                        {
                            employee.Fname = employee.Fname;
                            employee.Lname = employee.Lname;
                            employee.Address = employee.Address;
                            employee.Phonenum = employee.Phonenum;
                            employee.Email = employee.Email;
                            employee.RoleId = newRoleID;

                            //Assign fname and lname to empname, to use in email
                            string empname = employee.Lname + " " + employee.Fname;

                            Promotion promotion = new Promotion
                            {
                                EmpId = id,
                                Oldrole = actualID,
                                Newrole = newRoleID,
                                Datecreated = Convert.ToDateTime(date),
                            };

                            //If new role assigned is not manager, save the changes
                            db.Promotions.Add(promotion);
                            await db.SaveChangesAsync();
                            PromotionEmail(empname, employee.Email, actualRole, newRole);
                            return RedirectToAction("MyTeam");
                        }
                    }
                }
            }
            return View(model);
        }

        //Function to send email after promotion
        private void PromotionEmail(string empname, string email, string oldrole, string newrole)
        {
            MailMessage m = new MailMessage();
            SmtpClient sc = new SmtpClient();

            m.From = new MailAddress("moprofmoprof@gmail.com");
            m.To.Add(email);
            m.Subject = "Employee Management System";
            m.IsBodyHtml = true;
            StringBuilder msgBody = new StringBuilder();
            msgBody.Append("Dear " + empname + ",<br><br>");
            msgBody.Append("I wanted to personally inform you of an important update regarding your position at the company. After careful consideration, we have identified a new opportunity that aligns perfectly with your skills and expertise.<br><br>");
            msgBody.Append("I am pleased to inform you that as a "+ oldrole +" ,you have been selected for the position of "+ newrole + ".This role will allow you to leverage your strengths and make an even greater impact within our organization.<br><br>");
            msgBody.Append("Congratulations on your new position! We appreciate your dedication and look forward to witnessing your continued growth and success within our company.<br><br>");
            msgBody.Append("Should you have any questions or require further information, please feel free to reach out to me directly. I am here to assist you throughout this transition. <br><br>");
            msgBody.Append("Best regards, <br><br>");
            msgBody.Append("<br><br>");
            msgBody.Append("Employee Management System");
            m.Body = msgBody.ToString();
            sc.Host = "smtp.gmail.com";
            sc.Port = 587;
            sc.UseDefaultCredentials = false;
            sc.Credentials = new
            System.Net.NetworkCredential("moprofmoprof@gmail.com", "bfnjuwbainddmysg");
            sc.EnableSsl = true;
            sc.Send(m);
        }
    }
}
