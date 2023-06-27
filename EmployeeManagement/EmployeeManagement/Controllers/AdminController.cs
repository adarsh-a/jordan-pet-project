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
    public class AdminController : Controller
    {
        EmployeeMgtSystemEntities db = new EmployeeMgtSystemEntities();

        //Function for admin dashboard
        [HttpGet]
        public ActionResult AdminDashboard()
        {
            return View();
        }

        //Function to display roles//
        [HttpGet]
        public ActionResult Role()
        {
            return View(db.ROLEs.ToList());
        }

        //Function to create role//
        [HttpGet]
        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRole(ROLE model)
        {
            if (ModelState.IsValid)
            {
                //Retrieve role name to verify if the name already exists in database
                var CheckRole = from e in db.ROLEs
                                where e.role_name == model.role_name
                                select e.role_name;

                //Save the role 
                if (CheckRole.FirstOrDefault() == null)
                {
                    db.ROLEs.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Role");
                }
                else
                {
                    //Display error if the role name already exists
                    ModelState.AddModelError(nameof(model.role_name), "Role name already exists");
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        //Function to edit role//
        [HttpGet]
        public ActionResult EditRole(int id)
        {
            return View(db.ROLEs.Where(x => x.role_ID == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult EditRole(int id, ROLE role)
        {
            if (ModelState.IsValid)
            {
                //Retrieve the role name for the following id
                var ActualRole = from e in db.ROLEs
                                 where e.role_ID == role.role_ID
                                 select e.role_name;

                string actual_role = ActualRole.FirstOrDefault().ToString();

                //Check if the name entered already exists
                var CheckRole = from e in db.ROLEs
                                where e.role_name == role.role_name && e.role_name != actual_role
                                select e.role_name;

                //Save changes if the name does not exists
                if (CheckRole.FirstOrDefault() == null)
                {
                    db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Role");
                }
                else
                {
                    //Display error if the name already exists
                    ModelState.AddModelError(nameof(role.role_name), "Role name already exists");
                    return View(role);
                }
            }
            else
            {
                return View(role);
            }
        }

        //Function to delete role//
        [HttpGet]
        public ActionResult DeleteRole(int id)
        {
            return View(db.ROLEs.Where(x => x.role_ID == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult DeleteRole(int id, ROLE role)
        {
            if (ModelState.IsValid)
            {
                //Verify if this role is linked to emplyoyees
                var CheckRole = from e in db.Employees
                                where e.role_ID == id
                                select e.role_ID;

                //If role is not linked then proceed with delete operation otherwise display the error
                if (CheckRole.FirstOrDefault().ToString() == null)
                {
                    role = db.ROLEs.Where(x => x.role_ID == id).FirstOrDefault();
                    db.ROLEs.Remove(role);
                    db.SaveChanges();
                    return RedirectToAction("Role");
                }
                else
                {
                    return Content("WARNING: This role is linked with employees and therefore cannot be deleted from databse");
                }
            }
            else
            {
                return View(role);
            }
        }

        //Function to view all employees//
        [HttpGet]
        public ActionResult ViewEmployees()
        {
            return View(db.Employees.ToList());
        }

        //Function to create employee//
        [HttpGet]
        public ActionResult CreateEmployee()
        {
            //var getrolelist = db.ROLEs.ToList();
            var getrolelist = from role in db.ROLEs
                              where role.role_name != "Admin"
                              select role;

            SelectList list = new SelectList(getrolelist, "role_ID", "role_name");
            ViewBag.rolelistname = list;
            return View();
        }

        [HttpPost]
        public ActionResult CreateEmployee(EmployeeModels Emp)
        {
            if (ModelState.IsValid)
            {
                string pass = GeneratePassword();
                string Empstatus = "Active";

                //Verify all email addresses in database
                var CheckEmail = from e in db.Employees
                                 where e.email == Emp.email
                                 select e.email;

                //Verify all Phone numbers in database
                var CheckPhone = from e in db.Employees
                                 where e.phone_number == Emp.phone_number
                                 select e.phone_number;

                var employee = new Employee
                {
                    first_name = Emp.first_name,
                    last_name = Emp.last_name,
                    address = Emp.address,
                    phone_number = Emp.phone_number,
                    email = Emp.email,
                    //status = Emp.status,
                    status = Empstatus,
                    //password = Emp.password,
                    password = pass,
                    role_ID = Emp.role
                };

                //Verify if both results are true then proceed with insert operation
                if (CheckEmail.FirstOrDefault() == null && CheckPhone.FirstOrDefault() == null)
                {
                    RegistrationEmail(Emp.email.ToString(), pass);
                    db.Employees.Add(employee);
                    db.SaveChanges();
                    return RedirectToAction("ViewEmployees");
                }
                else
                {
                    //Verify if email is not unique then display the error
                    if (CheckEmail.FirstOrDefault() != null)
                    {
                        var getrolelist = db.ROLEs.ToList();
                        SelectList list = new SelectList(getrolelist, "role_ID", "role_name");
                        ViewBag.rolelistname = list;
                        ModelState.AddModelError(nameof(Emp.email), "Email address should be unique");
                    }

                    //Verify if phone number is not unique then display the error
                    if (CheckPhone.FirstOrDefault() != null)
                    {
                        var getrolelist = db.ROLEs.ToList();
                        SelectList list = new SelectList(getrolelist, "role_ID", "role_name");
                        ViewBag.rolelistname = list;
                        ModelState.AddModelError(nameof(Emp.phone_number), "Phone number should be unique");
                    }
                    return View(Emp);
                }
            }
            else
            {
                return View(Emp);
            }
        }

        //Function to Edit employee details as admin
        [HttpGet]
        public ActionResult EditEmployee(EditEmployeeModels model, int id)
        {
            //Retrieve roles from db and display in list
            var getrolelist = from role in db.ROLEs
                              where role.role_name != "Admin"
                              select role;
            SelectList list = new SelectList(getrolelist, "role_ID", "role_name");
            ViewBag.rolelistname = list;

            //Retrieve data for employee
            var employee = db.Employees.Single(r => r.emp_ID == id);

            //populate model with retrieved data
            if (employee != null)
            {
                model.emp_ID = employee.emp_ID;
                model.first_name = employee.first_name;
                model.last_name = employee.last_name;
                model.address = employee.address;
                model.phone_number = employee.phone_number;
                model.email = employee.email;
                model.status = employee.status;
                model.password = employee.password;
                model.role_ID = employee.role_ID;
                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult EditEmployee(EditEmployeeModels model)
        {
            if (ModelState.IsValid)
            {
                //Retrieve the employee email
                var CheckEmployeeEmail = from e in db.Employees
                                         where e.emp_ID == model.emp_ID
                                         select e.email;

                var current_email = CheckEmployeeEmail.FirstOrDefault().ToString();

                //Retrieve the employee phone number
                var CheckEmployeePhone = from e in db.Employees
                                         where e.emp_ID == model.emp_ID
                                         select e.phone_number;

                var current_phone = CheckEmployeePhone.FirstOrDefault().ToString();

                //Verify all email excluding the actual email for the employee
                var CheckEmail = from e in db.Employees
                                 where e.email == model.email && e.email != current_email
                                 select e.email;

                //Verify all phone number excluding the actual phone number for the employee
                var checkPhone = from e in db.Employees
                                 where e.phone_number == model.phone_number && e.phone_number != current_phone
                                 select e.phone_number;

                Employee employee = new Employee
                {
                    emp_ID = model.emp_ID,
                    first_name = model.first_name,
                    last_name = model.last_name,
                    email = model.email,
                    address = model.address,
                    phone_number = model.phone_number,
                    status = model.status,
                    password = model.password,
                    role_ID = model.role_ID
                };

                //Verify if both results are true, then proceed with update 
                if (CheckEmail.FirstOrDefault() == null && checkPhone.FirstOrDefault() == null)
                {
                    db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ViewEmployees");
                }
                else
                {
                    //Verify if email is not validated then display the error
                    if (CheckEmail.FirstOrDefault() != null)
                    {
                        var getrolelist = db.ROLEs.ToList();
                        SelectList list = new SelectList(getrolelist, "role_ID", "role_name");
                        ViewBag.rolelistname = list;
                        ModelState.AddModelError(nameof(model.email), "Email address should be unique");
                    }

                    //Verify if phone number is not validated then display the error
                    if (checkPhone.FirstOrDefault() != null)
                    {
                        var getrolelist = db.ROLEs.ToList();
                        SelectList list = new SelectList(getrolelist, "role_ID", "role_name");
                        ViewBag.rolelistname = list;
                        ModelState.AddModelError(nameof(model.phone_number), "Phone number should be unique");
                    }

                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        //Function to delete employee
        [HttpGet]
        public ActionResult DeleteEmployee(int id)
        {
            return View(db.Employees.Where(x => x.emp_ID == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult DeleteEmployee(Employee employee, int id)
        {
            if (ModelState.IsValid)
            {
                //Check if the manager is already assigned to a team
                var CheckIfManagerExists = db.TEAMS1.Where(x => x.manager_ID == id).FirstOrDefault();

                //Check if the user is already assigned in a team
                var CheckIfEmployeeExists = db.TMEMBERS.Where(x => x.emp_ID == id).FirstOrDefault();

                //Select the role name from the employee
                var result = from emp in db.Employees
                             join role in db.ROLEs on emp.role_ID equals role.role_ID
                             where emp.emp_ID == id
                             select role.role_name;

                //Check if the employee role

                if (result.FirstOrDefault() == "Manager")
                {
                    if (CheckIfManagerExists == null)
                    {
                        employee = db.Employees.Where(x => x.emp_ID == id).FirstOrDefault();
                        db.Employees.Remove(employee);
                        db.SaveChanges();
                        return RedirectToAction("ViewEmployees");
                    }
                    else
                    {
                        return Content("This employee cannot be deleted");
                    }
                }

                if (result.FirstOrDefault() != null && result.FirstOrDefault() != "Manager" && result.FirstOrDefault() != "Admin")
                {
                    if (CheckIfEmployeeExists == null)
                    {
                        employee = db.Employees.Where(x => x.emp_ID == id).FirstOrDefault();
                        db.Employees.Remove(employee);
                        db.SaveChanges();
                        return RedirectToAction("ViewEmployees");
                    }
                    else
                    {
                        return Content("This employee cannot be deleted");
                    }
                }

                return View(employee);
            }
            else
            {
                return View(employee);
            }
        }

        //Function to view all teams
        [HttpGet]
        public ActionResult ViewTeams()
        {
            return View(db.TEAMS1.ToList());
        }

        //Function to create teams
        [HttpGet]
        public ActionResult CreateTeams()
        {
            //Select only managers who are not assigned to any team
            var getrolelist = from employee in db.Employees
                              join role in db.ROLEs on employee.role_ID equals role.role_ID
                              where role.role_name == "Manager" && !db.TEAMS1.Select(team => team.manager_ID).Contains(employee.emp_ID)
                              select employee;

            //Create a selectlist from the data obtained
            SelectList list = new SelectList(getrolelist, "emp_ID", "first_name");
            ViewBag.getrolelistname = list;
            return View();
        }

        [HttpPost]
        public ActionResult CreateTeams(TEAM1 model)
        {
            //Verify if team name exists in database
            var checkTeamName = from t in db.TEAMS1
                                where t.teams_name == model.teams_name
                                select t.teams_name;

            //Save the changes if team name does not exists else display error
            if (checkTeamName.FirstOrDefault() == null)
            {
                TEAM1 team = new TEAM1
                {
                    teams_name = model.teams_name,
                    manager_ID = model.manager_ID
                };

                db.TEAMS1.Add(team);
                db.SaveChanges();
                return RedirectToAction("ViewTeams");
            }
            else
            {
                var getrolelist = from employee in db.Employees
                                  join role in db.ROLEs on employee.role_ID equals role.role_ID
                                  where role.role_name == "Manager" && !db.TEAMS1.Select(team => team.manager_ID).Contains(employee.emp_ID)
                                  select employee;

                SelectList list = new SelectList(getrolelist, "emp_ID", "first_name");
                ViewBag.getrolelistname = list;

                ModelState.AddModelError(nameof(TEAM1.teams_name), "Team name already exists");

                return View(model);
            }
        }

        //Function to edit teams
        [HttpGet]
        public ActionResult EditTeams(int id)
        {
            TEAM1 model = new TEAM1();

            //Retrieve and diaplay managers who are not assigned to any team
            var getrolelist = from employee in db.Employees
                              join role in db.ROLEs on employee.role_ID equals role.role_ID
                              where role.role_name == "Manager" &&
                              !db.TEAMS1.Where(t => t.teams_ID != id).Select(t => t.manager_ID).Contains(employee.emp_ID)
                              select employee;

            //Create a seleclist from data obtained
            SelectList list = new SelectList(getrolelist, "emp_ID", "first_name");
            ViewBag.getrolelistname = list;

            var team = db.TEAMS1.SingleOrDefault(r => r.teams_ID == id);

            if (team != null)
            {

                model.teams_ID = team.teams_ID;
                model.teams_name = team.teams_name;
                model.manager_ID = team.manager_ID;
                return View(model);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditTeams(int id, TEAM1 model)
        {
            var getrolelist = from employee in db.Employees
                              join role in db.ROLEs on employee.role_ID equals role.role_ID
                              where role.role_name == "Manager" &&
                              !db.TEAMS1.Where(t => t.teams_ID != id).Select(t => t.manager_ID).Contains(employee.emp_ID)
                              select employee;

            SelectList list = new SelectList(getrolelist, "emp_ID", "first_name");
            ViewBag.getrolelistname = list;

            if (ModelState.IsValid)
            {
                //Select all team names 
                var actual_teamname = from t in db.TEAMS1
                                      where t.teams_ID == id
                                      select t.teams_name;

                //Verify if the team name is unique
                var checkNames = from t in db.TEAMS1
                                 where t.teams_name == model.teams_name && t.teams_name != actual_teamname.FirstOrDefault()
                                 select t.teams_name;

                if (checkNames.FirstOrDefault() == null)
                {
                    TEAM1 team = new TEAM1
                    {

                        teams_ID = model.teams_ID,
                        teams_name = model.teams_name,
                        manager_ID = model.manager_ID

                    };

                    //Save the changes
                    db.Entry(team).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ViewTeams");
                }
                else
                {
                    //Display error id team name is not unique
                    ModelState.AddModelError(nameof(TEAM1.teams_name), "Team name already exists");
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        //Function to delete teams
        [HttpGet]
        public ActionResult DeleteTeams(int id)
        {
            return View(db.TEAMS1.Where(x => x.teams_ID == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult DeleteTeams(int id, TEAM1 model)
        {
            //Select the team id
            var team = db.TEAMS1.Where(x => x.teams_ID == id).FirstOrDefault();

            //Verify if the team id is not present in the table team members
            var checkTeam = db.TMEMBERS.Where(x => x.teams_ID == id).FirstOrDefault();

            if (checkTeam == null)
            {
                db.TEAMS1.Remove(team);
                db.SaveChanges();
                return RedirectToAction("ViewTeams");
            }
            else
            {
                return Content("This team cannot be deleted");
            }
        }

        //Function to display team members
        [HttpGet]
        public ActionResult ViewTeamMembers(int id)
        {
            ViewBag.Teamid = id;

            //Select all members for each team
            var results = from role in db.ROLEs
                          join employee in db.Employees on role.role_ID equals employee.role_ID
                          join tmember in db.TMEMBERS on employee.emp_ID equals tmember.emp_ID
                          join team in db.TEAMS1 on tmember.teams_ID equals team.teams_ID
                          where tmember.teams_ID == id
                          select new AddMembersModels
                          {
                              emp_ID = employee.emp_ID,
                              first_name = employee.first_name,
                              last_name = employee.last_name,
                              email = employee.email,
                              role_name = role.role_name
                          };

            return View(results);
        }

        //Function to add members to a team
        [HttpGet]
        public ActionResult AddTeamMembers(int id)
        {
            //Select all employees who are not associated to any team and populate list in selectlist
            var getemp = from employee in db.Employees
                         join role in db.ROLEs on employee.role_ID equals role.role_ID
                         where role.role_name != "Admin" && role.role_name != "Manager" && !db.TMEMBERS.Select(tm => tm.emp_ID).Contains(employee.emp_ID)
                         select employee;

            SelectList emplist = new SelectList(getemp, "emp_ID", "first_name");
            ViewBag.getempname = emplist;
            return View();
        }

        [HttpPost]
        public ActionResult AddTeamMembers(int id, FormCollection data)
        {

            var getemp = from employee in db.Employees
                         join role in db.ROLEs on employee.role_ID equals role.role_ID
                         where role.role_name != "Admin" && role.role_name != "Manager" && !db.TMEMBERS.Select(tm => tm.emp_ID).Contains(employee.emp_ID)
                         select employee;

            SelectList emplist = new SelectList(getemp, "emp_ID", "first_name");
            ViewBag.getempname = emplist;

            TMEMBER model = new TMEMBER();

            if (ModelState.IsValid)
            {
                var emp = data["emp_ID"];
                string[] employee = emp.Split(',');

                foreach (string s in employee)
                {
                    TMEMBER team = new TMEMBER
                    {
                        teams_ID = id,
                        emp_ID = Convert.ToInt32(s)
                    };

                    db.TMEMBERS.Add(team);
                }
                db.SaveChanges();
                return RedirectToAction("ViewTeams");
            }
            else
            {
                return View(model);
            }
        }

        //Function to remove employee from team
        [HttpGet]
        public ActionResult RemoveEmployee(int id)
        {
            var employee = (from e in db.Employees
                            where e.emp_ID == id
                            select e).FirstOrDefault();

            return View(employee);
        }

        [HttpPost]
        public ActionResult RemoveEmployee(int id, TMEMBER model)
        {
            var member = db.TMEMBERS.Where(x => x.emp_ID == id).FirstOrDefault();

            //Remove member from a team
            if (member != null)
            {
                db.TMEMBERS.Remove(member);
                db.SaveChanges();
                return RedirectToAction("ViewTeams");
            }
            else
            {
                return View();
            }
        }

        //Function to auto-generate password for registration of employees
        public string GeneratePassword()
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < 8; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }

            return sb.ToString();
        }

        //Function to send user email after registration
        private void RegistrationEmail(string email, string password)
        {
            MailMessage m = new MailMessage();
            SmtpClient sc = new SmtpClient();

            m.From = new MailAddress("moprofmoprof@gmail.com");
            m.To.Add(email);
            m.Subject = "Employee Management System";
            m.IsBodyHtml = true;
            StringBuilder msgBody = new StringBuilder();
            msgBody.Append("Dear " + email + ",<br><br>");
            msgBody.Append("Welcome to Employee Management System <br><br>");
            msgBody.Append("Kindly note that you account has been successfully created, <br><br>");
            msgBody.Append("Please use the given email address and password in order to access your account. <br><br>");
            msgBody.Append("Your email: " + email + " and your password is: " + password + "<br><br>");
            msgBody.Append("You are also requested to change your password once you logged into your account. <br><br>");
            msgBody.Append("Thank you <br><br>");
            msgBody.Append("Kind regards, <br><br>");
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