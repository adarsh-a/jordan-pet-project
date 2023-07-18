using EmployeeMgtCore.DataDB;
using EmployeeMgtCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace EmployeeMgtCore.Controllers
{
    public class AdminController : Controller
    {
        //Database connection
        EmployeeMgtCoreContext db = new EmployeeMgtCoreContext();

        //Function for admin dashboard
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        //Function to view all roles
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ViewRoleList()
        {
            //Retrieve number of roles and save in viewbag
            var count = db.Roles.ToList().Count();
            ViewBag.countroles = count.ToString();

            return View(db.Roles.ToList());
        }

        //Function to create role
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(Role model)
        {
            if (ModelState.IsValid)
            {
                //Verify if the role name exists in database
                var checkRoleNames = from r in db.Roles
                                     where r.Rolename == model.Rolename
                                     select r.Rolename;

                //If role name already exists, display error else create new role
                if (checkRoleNames.FirstOrDefault() != null)
                {
                    ModelState.AddModelError(nameof(model.Rolename), "Role already exists");
                    return View(model);
                }
                else
                {
                    TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
                    string rname = textInfo.ToTitleCase(model.Rolename.ToString());

                    Role role = new Role
                    {
                        Rolename = rname
                    };

                    db.Roles.Add(role);
                    await db.SaveChangesAsync();
                    return RedirectToAction("ViewRoleList");
                }
            }
            else
            {
                return View(model);
            } 
        }

        //Function to edit role
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditRole(int id)
        {
            Role model = new Role();

            var role = db.Roles.SingleOrDefault(r => r.RoleId == id);

            if (role != null)
            {
                model.RoleId = role.RoleId;
                model.Rolename = role.Rolename;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(int id, Role model)
        {
            if (ModelState.IsValid)
            {
                var role = await db.Roles.FindAsync(id);

                //Retrieve the actual role name
                var actualRole = from r in db.Roles
                                 where r.RoleId == id
                                 select r.Rolename;

                //Check if the name entered is unique
                var checkRole = from r in db.Roles
                                where r.Rolename == model.Rolename && r.Rolename != actualRole.FirstOrDefault()
                                select r.Rolename;

                //If name entered is unique then save it else display error
                if (role != null)
                {
                    if (checkRole.FirstOrDefault() == null)
                    {
                        role.Rolename = model.Rolename;
                        await db.SaveChangesAsync();
                        return RedirectToAction("ViewRoleList");
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.Rolename), "Role already exists");
                        return View(model);
                    }
                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        //Function to delete role
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DeleteRole(int id)
        {
            Role model = new Role();

            var role = db.Roles.SingleOrDefault(r => r.RoleId == id);

            if(role != null)
            {
                model.RoleId = role.RoleId;
                model.Rolename = role.Rolename;
                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(int id, Role model)
        {
            if (!ModelState.IsValid)
            {
                var role = await db.Roles.FindAsync(id);

                //Check if the role we want to delete is associated to an employee
                var checkIfUsed = from e in db.Employees
                                  where e.RoleId == id
                                  select e.RoleId;

                if (role != null)
                {
                    //If role is not  associated to any employee, delete it else prompt an error
                    if (checkIfUsed.FirstOrDefault() == null)
                    {
                        db.Roles.Remove(role);
                        await db.SaveChangesAsync();
                        return RedirectToAction("ViewRoleList");
                    }
                    else
                    {
                        return Content("WARNING: This role cannot be deleted from database");
                    }
                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        //Function to View employee
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ViewEmployee()
        {
            //Retrieve all employees except the admin and display
            var employees = from role in db.Roles
                            join employee in db.Employees on role.RoleId equals employee.RoleId
                            where role.Rolename != "Admin"
                            select new EmployeeListModel
                            {
                                emp_ID = employee.EmpId,
                                fname = employee.Fname,
                                lname = employee.Lname,
                                address = employee.Address,
                                phonenum = employee.Phonenum,
                                email = employee.Email,
                                rolename = role.Rolename
                            };

            return View(employees);
        }

        //Function to create employee
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateEmployee()
        {
            CreateEmployeeModel model = new CreateEmployeeModel();

            //Select all roles except the admin
            var getrolelist = from role in db.Roles
                              where role.Rolename != "Admin"
                              select role;

            //Create select list with the roles name
            List<SelectListItem> list = new List<SelectListItem>
                (
                    getrolelist.Select(role => new SelectListItem
                    {
                        Value = role.RoleId.ToString(),
                        Text = role.Rolename
                    })
                );

            ViewBag.rolelistname = list;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeModel model)
        {
            var getrolelist = from role in db.Roles
                              where role.Rolename != "Admin"
                              select role;

            List<SelectListItem> list = new List<SelectListItem>
                (
                    getrolelist.Select(role => new SelectListItem
                    {
                        Value = role.RoleId.ToString(),
                        Text = role.Rolename
                    })
                );

            ViewBag.rolelistname = list;

            if (ModelState.IsValid)
            {

                //Verify if the phone number entered already exists
                var checkPhone = from p in db.Employees
                                 where p.Phonenum == model.phonenum
                                 select p.Phonenum;

                //Verify if the email entered already exists
                var checkEmail = from e in db.Employees
                                 where e.Email == model.email
                                 select e.Email;

                //Store the value for generate password
                string pass = GeneratePassword();

                //If phone and email are unique, create new employee else display error
                if (checkPhone.FirstOrDefault() == null && checkEmail.FirstOrDefault() == null)
                {
                    Employee emp = new Employee
                    {
                        Fname = model.fname,
                        Lname = model.lname,
                        Address = model.address,
                        Phonenum = model.phonenum,
                        Email = model.email,
                        Password = pass,
                        RoleId = model.role_ID
                    };

                    db.Employees.Add(emp);
                    RegistrationEmail(model.email, pass);
                    await db.SaveChangesAsync();
                    return RedirectToAction("ViewEmployee");
                }
                else
                {
                    if (checkPhone.FirstOrDefault() != null)
                    {
                        ModelState.AddModelError(nameof(model.phonenum), "Phone number should be unique");
                    }

                    if (checkEmail.FirstOrDefault() != null)
                    {
                        ModelState.AddModelError(nameof(model.email), "Email address should be unique");
                    }

                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        //Function to edit employee details
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            CreateEmployeeModel model = new CreateEmployeeModel();

            var getrolelist = from role in db.Roles
                              where role.Rolename != "Admin"
                              select role;

            List<SelectListItem> list = new List<SelectListItem>
                (
                    getrolelist.Select(role => new SelectListItem
                    {
                        Value = role.RoleId.ToString(),
                        Text = role.Rolename
                    })
                );

            ViewBag.rolelistname = list;

            //Retrieve employee details and map it on the model
            var currentEmployee = db.Employees.Find(id);

            if (currentEmployee != null)
            {
                model.emp_ID = currentEmployee.EmpId;
                model.fname = currentEmployee.Fname;
                model.lname = currentEmployee.Lname;
                model.address = currentEmployee.Address;
                model.phonenum = currentEmployee.Phonenum;
                model.email = currentEmployee.Email;
                model.role_ID = currentEmployee.RoleId;

                return View(model);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(int id, CreateEmployeeModel model)
        {
            var getrolelist = from role in db.Roles
                              where role.Rolename != "Admin"
                              select role;

            List<SelectListItem> list = new List<SelectListItem>
                (
                    getrolelist.Select(role => new SelectListItem
                    {
                        Value = role.RoleId.ToString(),
                        Text = role.Rolename
                    })
                );

            ViewBag.rolelistname = list;

            if (ModelState.IsValid)
            {
                //Retrieve the employee phone number
                var empPhone = from emp in db.Employees
                               where emp.EmpId == id
                               select emp.Phonenum;

                //Retrieve the employee email address
                var empEmail = from emp in db.Employees
                               where emp.EmpId == id
                               select emp.Email;

                //Check if phone number entered is unique
                var checkPhone = from emp in db.Employees
                                 where emp.Phonenum == model.phonenum && emp.Phonenum != empPhone.FirstOrDefault()
                                 select emp.Phonenum;

                //Check if email address entered is unique
                var checkEmail = from emp in db.Employees
                                 where emp.Email == model.email && emp.Email != empEmail.FirstOrDefault()
                                 select emp.Email;

                //If email and phone is unique then edit the user info else display error
                if (checkPhone.FirstOrDefault() == null && checkEmail.FirstOrDefault() == null)
                {
                    var employee = await db.Employees.FindAsync(id);

                    if (employee != null)
                    {
                        employee.Fname = model.fname;
                        employee.Lname = model.lname;
                        employee.Address = model.address;
                        employee.Phonenum = model.phonenum;
                        employee.Email = model.email;
                        employee.RoleId = model.role_ID;

                        await db.SaveChangesAsync();
                        return RedirectToAction("ViewEmployee");
                    }
                    else
                    {
                        return View(model);
                    }
                }
                else
                {
                    if (checkPhone.FirstOrDefault() != null)
                    {
                        ModelState.AddModelError(nameof(model.phonenum), "Phone number should be unique");
                    }
                    if (checkEmail.FirstOrDefault() != null)
                    {
                        ModelState.AddModelError(nameof(model.email), "Email address should be unique");
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DeleteEmployee(int id)
        {
            CreateEmployeeModel model = new CreateEmployeeModel();

            //Retrieve the employee details and map on the model
            var employee = db.Employees.Find(id);

            if (employee != null)
            {
                model.emp_ID = employee.EmpId;
                model.fname = employee.Fname;
                model.lname = employee.Lname;
                model.address = employee.Address;
                model.phonenum = employee.Phonenum;
                model.email = employee.Email;
                model.role_ID = employee.RoleId;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id, CreateEmployeeModel model)
        {
            //Retrieve employee details based on their id
            var employee = await db.Employees.FindAsync(id);

            if (employee != null)
            {
                //Verify if employee is in a team
                var emp = db.Tmembers.Where(x => x.EmpId == id).FirstOrDefault();

                //Verify if the manager is already associated to a team
                var manager = db.Teams.Where(x => x.ManagerId == id).FirstOrDefault();

                // retrieve role name of the employee
                var result = from em in db.Employees
                             join role in db.Roles on em.RoleId equals role.RoleId
                             where em.EmpId == id
                             select role.Rolename;

                //If the role name is manager, verify if he/she is associated to a team and remove the manager
                if (result.FirstOrDefault() == "Manager")
                {
                    if (manager == null)
                    {
                        db.Employees.Remove(employee);
                        await db.SaveChangesAsync();
                        return RedirectToAction("ViewEmployee");
                    }
                    else
                    {
                        return Content("This manager cannot be deleted");
                    }
                }

                //If role is is admin and manager, verify if employee is in a team then proceed or display error
                if (result.FirstOrDefault() != null && result.FirstOrDefault() != "Manager" && result.FirstOrDefault() != "Admin")
                {
                    if (emp == null)
                    {
                        db.Employees.Remove(employee);
                        await db.SaveChangesAsync();
                        return RedirectToAction("ViewEmployee");
                    }
                    else
                    {
                        return Content("This employee is a member of a team and cannot be deleted");
                    }
                } 
            }
            return View(model);
        }

        //Function to View team list
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ViewTeamList() 
        {
            //Select team name and manager name
            var result = from team in db.Teams
                         join employee in db.Employees on team.ManagerId equals employee.EmpId
                         select new CreateTeamModel
                         {
                             Team_ID = team.TeamId,
                             teamname = team.Teamname,
                             mangername = employee.Fname +" "+ employee.Lname
                         };

            return View(result);
        }

        //Function to create team
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateTeam()
        {
            Team model = new Team();

            //Retrieve all managers who are not associated to any team
            var getmanagerlist = from employee in db.Employees
                                join role in db.Roles on employee.RoleId equals role.RoleId
                                where role.Rolename == "Manager" && !db.Teams.Select(team => team.ManagerId).Contains(employee.EmpId)
                                select employee;

            //Create select list with all the managers
            List<SelectListItem> list = new List<SelectListItem>
                (
                    getmanagerlist.Select(employee => new SelectListItem
                    {
                        Value = employee.EmpId.ToString(),
                        Text = employee.Fname + " " + employee.Lname + " " + "[ " + employee.Email + " ]"
                    })
                );

            ViewBag.managerlistname = list;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam(Team model)
        {
            //Retrieve and display all managers who are not linked to any team
            var getmanagerlist = from employee in db.Employees
                                 join role in db.Roles on employee.RoleId equals role.RoleId
                                 where role.Rolename == "Manager" && !db.Teams.Select(team => team.ManagerId).Contains(employee.EmpId)
                                 select employee;

            List<SelectListItem> list = new List<SelectListItem>
                (
                    getmanagerlist.Select(employee => new SelectListItem
                    {
                        Value = employee.EmpId.ToString(),
                        Text = employee.Fname + " " + employee.Lname + " " + "[ " + employee.Email + " ]"
                    })
                );

            ViewBag.managerlistname = list;

            if (ModelState.IsValid)
            {
                //Verify if team name is unique else display error
                var checkTeamName = from t in db.Teams
                                    where t.Teamname == model.Teamname
                                    select t.Teamname;

                if (checkTeamName.FirstOrDefault() == null)
                {
                    Team team = new Team
                    {
                        Teamname = model.Teamname,
                        ManagerId = model.ManagerId
                    };

                    db.Teams.Add(team);
                    await db.SaveChangesAsync();
                    return RedirectToAction("ViewTeamList");
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Teamname), "Team already exists");
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        //Function to edit team details
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditTeamDetails(int id)
        {
            Team model = new Team();

            //Retriueve all managers who are not associated to any team
            var getmanagerlist = from employee in db.Employees
                              join role in db.Roles on employee.RoleId equals role.RoleId
                              where role.Rolename == "Manager" &&
                              !db.Teams.Where(t => t.TeamId != id).Select(t => t.ManagerId).Contains(employee.EmpId)
                              select employee;

            List<SelectListItem> list = new List<SelectListItem>
                (
                    getmanagerlist.Select(employee => new SelectListItem
                    {
                        Value = employee.EmpId.ToString(),
                        Text = employee.Fname + " " + employee.Lname + " " + "[ " + employee.Email + " ]"
                    })
                );

            ViewBag.managerlistname = list;

            var currentTeam = db.Teams.Find(id);

            if (currentTeam != null)
            {
                model.TeamId = currentTeam.TeamId;
                model.Teamname = currentTeam.Teamname;
                model.ManagerId = currentTeam.ManagerId;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditTeamDetails(int id, Team model)
        {
            var getmanagerlist = from employee in db.Employees
                                 join role in db.Roles on employee.RoleId equals role.RoleId
                                 where role.Rolename == "Manager" &&
                                 !db.Teams.Where(t => t.TeamId != id).Select(t => t.ManagerId).Contains(employee.EmpId)
                                 select employee;

            List<SelectListItem> list = new List<SelectListItem>
                (
                    getmanagerlist.Select(employee => new SelectListItem
                    {
                        Value = employee.EmpId.ToString(),
                        Text = employee.Fname + " " + employee.Lname + " " + "[ " + employee.Email + " ]"
                    })
                );

            ViewBag.managerlistname = list;

            if (ModelState.IsValid)
            {
                var team = await db.Teams.FindAsync(id);

                //Retrieve the actaul name of the team
                var actualname = from t in db.Teams
                                 where t.TeamId == id
                                 select t.Teamname;

                //Verify if name entered is unique
                var checkTeams = from t in db.Teams
                                 where t.Teamname == model.Teamname && t.Teamname != actualname.FirstOrDefault()
                                 select t.Teamname;

                //If team is unique, proceed else display error
                if (team != null)
                {
                    if (checkTeams.FirstOrDefault() == null)
                    {
                        team.TeamId = model.TeamId;
                        team.Teamname = model.Teamname;
                        team.ManagerId = model.ManagerId;

                        await db.SaveChangesAsync();
                        return RedirectToAction("ViewTeamList");
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.Teamname), "Team already exists");
                        return View(model);
                    }
                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        //Function to delete team
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            DeleteTeamModel model = new DeleteTeamModel();

            var team = await db.Teams.FindAsync(id);

            //Display team details
            if (team != null)
            {
                var result = (from employee in db.Employees
                             where employee.EmpId == team.ManagerId
                             select new DeleteTeamModel
                             {
                                 teamID = team.TeamId,
                                 teamname = team.Teamname,
                                 managername = employee.Fname + " " + employee.Lname
                             }).FirstOrDefault();

                return View(result);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTeam(int id, DeleteTeamModel model)
        {
            var team = await db.Teams.FindAsync(id);

            if (team != null)
            {
                //Verify if the team we want to delete contains team members
                var checkTeam = from t in db.Tmembers
                                where t.TeamId == id
                                select t.TeamId;

                //If the team already has team members, display error else delete the team
                if(checkTeam.FirstOrDefault() == null)
                {
                    db.Teams.Remove(team);
                    await db.SaveChangesAsync();
                    return RedirectToAction("ViewTeamList");
                }
                else
                {
                    return Content("This team cannot be deleted");
                }
            }
            else
            {
                return View(model);
            }
        }

        //Function to manage team members
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ManageTeamMembers(int id)
        {
            ViewBag.Teamid = id;

            //Retrieve number of employees in team and store in viewbag
            var count = db.Tmembers.Where(e=> e.TeamId == id).ToList().Count();
            ViewBag.countmembers = count.ToString();

            //Retrieve all employees for this team
            var result = from employee in db.Employees
                         join role in db.Roles on employee.RoleId equals role.RoleId
                         join tmember in db.Tmembers on employee.EmpId equals tmember.EmpId
                         where tmember.TeamId == id
                         select new ManageMembersModel
                         {
                             memberId = employee.EmpId,
                             membername = employee.Fname + " " + employee.Lname,
                             email = employee.Email,
                             rolename = role.Rolename
                         };

            return View(result);
        }

        //Function to add team members 
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddMember(int id)
        {
            Tmember model = new Tmember();

            //Retrieve all employees who are not in a team yet
            var getemployeelist = from employee in db.Employees
                                  join role in db.Roles on employee.RoleId equals role.RoleId
                                  where role.Rolename != "Admin" && role.Rolename != "Manager" && !db.Tmembers.Select(tm => tm.EmpId).Contains(employee.EmpId)
                                  select employee;

            //Create a select list with all the employees
            List<SelectListItem> list = new List<SelectListItem>
                (
                    getemployeelist.Select(employee => new SelectListItem
                    {
                        Value = employee.EmpId.ToString(),
                        Text = employee.Fname + " " + employee.Lname + " " + "[ " + employee.Email + " ]"
                    })
                );

            ViewBag.employeelistname = list;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int id, Tmember model, IFormCollection data)
        {
            var getemployeelist = from employee in db.Employees
                                  join role in db.Roles on employee.RoleId equals role.RoleId
                                  where role.Rolename != "Admin" && role.Rolename != "Manager" && !db.Tmembers.Select(tm => tm.EmpId).Contains(employee.EmpId)
                                  select employee;

            List<SelectListItem> list = new List<SelectListItem>
                (
                    getemployeelist.Select(employee => new SelectListItem
                    {
                        Value = employee.EmpId.ToString(),
                        Text = employee.Fname + " " + employee.Lname + " " + "[ " + employee.Email + " ]"
                    })
                );

            ViewBag.employeelistname = list;

            if (ModelState.IsValid)
            {
                string empID = data["EmpId"];
                //If multiple users have been selected
                string[] employee = empID.Split(',');

                foreach ( string emp in employee )
                {
                    Tmember member = new Tmember {

                        TeamId = id,
                        EmpId = Convert.ToInt32(emp)
                    };

                    db.Tmembers.Add(member);
                }
                //save the changes
                await db.SaveChangesAsync();
                return RedirectToAction("ViewTeamList");
            }
            else
            {
                return View(model);
            }
        }

        //Function to remove employee from team
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult RemoveEmployee(int id)
        {
            //Display all the employees in the team and map on the model
            var result = (from employee in db.Employees
                         join role in db.Roles on employee.RoleId equals role.RoleId
                         join tmember in db.Tmembers on employee.EmpId equals tmember.EmpId
                         join team in db.Teams on tmember.TeamId equals team.TeamId
                         where employee.EmpId == id
                         select new RemoveEmployeeModel
                         {
                             empID = employee.EmpId,
                             empname = employee.Fname + " " + employee.Lname,
                             email = employee.Email,
                             rolename = role.Rolename,
                             teamname = team.Teamname
                         }).FirstOrDefault();

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveEmployee(int id, RemoveEmployeeModel model)
        {
            if (ModelState.IsValid)
            {
                //Verify the employee
                var member = await db.Tmembers.FirstOrDefaultAsync(emp => emp.EmpId == id);

                //If the employee is in a the team, remove the employee from the team
                if (member != null)
                {
                    db.Tmembers.Remove(member);
                    await db.SaveChangesAsync();
                    return RedirectToAction("ViewTeamList");
                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        //Function to auto generate password
        public string GeneratePassword()
        {
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@*%$£";

            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < 8; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }
            return sb.ToString();
        }

        //Function to send email on registration
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
