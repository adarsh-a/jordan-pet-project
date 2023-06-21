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

        //Function to test auto-generated password
        //public ActionResult PasswordTest()
        //{
        //    ViewBag.test = GeneratePassword();
        //    return View();
        //}

        //Function for login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModels model)
        {
            var CheckEmployee = db.Employees.Where(x => x.email.Equals(model.username) && x.password.Equals(model.password)).FirstOrDefault();

            if(CheckEmployee != null)
            {
                var EmpDetails = from e in db.Employees
                                 join r in db.ROLEs on e.role_ID equals r.role_ID
                                 where e.email == CheckEmployee.email
                                 select r.role_name;

                Session["email"] = CheckEmployee.email.ToString();
                Session["eid"] = CheckEmployee.emp_ID.ToString();
                Session["role"] = EmpDetails.FirstOrDefault().ToString();

                if(Session["Role"].ToString() == "Admin")
                {
                    return RedirectToAction("AdminDashboard");
                }
                else if (Session["Role"].ToString() == "Manager")
                {
                    return RedirectToAction("ManagerDashboard");
                }
                else if (Session["Role"].ToString() != "Manager" || Session["Role"].ToString() != "Admin")
                {
                    return RedirectToAction("EmployeeDashboard");
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

        //Function to sign user out
        public ActionResult Signout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }

        //Function to display roles//
        //[Authorize(Roles ="Admin")]
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
                //var role = new ROLE
                //{
                //  role_name = model.role_name
                //};
                var CheckRole = from e in db.ROLEs
                                 where e.role_name == model.role_name
                                 select e.role_name;

                if(CheckRole.FirstOrDefault() == null)
                {
                    db.ROLEs.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Role");
                }
                else
                {
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
            return View(db.ROLEs.Where(x=>x.role_ID == id).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult EditRole(int id, ROLE role)
        {
            if (ModelState.IsValid)
            {
                var ActualRole = from e in db.ROLEs
                                where e.role_ID == role.role_ID
                                select e.role_name;

                string actual_role = ActualRole.FirstOrDefault().ToString();

                var CheckRole = from e in db.ROLEs
                                where e.role_name == role.role_name && e.role_name != actual_role 
                                select e.role_name;

                if(CheckRole.FirstOrDefault() == null)
                {
                    db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Role");
                }
                else
                {
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
                                 select e.first_name;

                //If role is not linked then proceed with delete operation otherwise display the error
                if (CheckRole.FirstOrDefault() == null)
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
            var getrolelist = db.ROLEs.ToList();
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
                if(CheckEmail.FirstOrDefault() == null && CheckPhone.FirstOrDefault() == null)
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
            var getrolelist = db.ROLEs.ToList();
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
                //var CheckIfEmployeeExists = db.TEAMs.SqlQuery("select emp_ID, manager_Id from TEAM where emp_ID == " + id + "or manager_ID == " + id);
                employee = db.Employees.Where(x => x.emp_ID == id).FirstOrDefault();
                db.Employees.Remove(employee);
                db.SaveChanges();
                return RedirectToAction("ViewEmployees");
            }
            else
            {
                return View(employee);
            }
        }

        //Function to view users//
        [HttpGet]
        public ActionResult ViewProfiles()
        {
            return View(db.Employees.ToList());
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

            db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            if (Session["role"].ToString() == "Manager")
            {
                return RedirectToAction("ManagerDashboard");
            }
            else if (Session["role"].ToString() != "Manager" && Session["role"].ToString() != "Admin")
            {
                return RedirectToAction("EmployeeDashboard");
            }

            return View(model);
        }

        //Function to view team by admin
        [HttpGet]
        public ActionResult ViewTeam(TEAM team)
        {
            return View(db.TEAMs.ToList());
        }

        //Function to create team by admin
        [HttpGet]
        public ActionResult CreateTeam() 
        {
            var getrolelist = db.Employees.Where(x => x.role_ID.Equals(7));
            SelectList list = new SelectList(getrolelist, "emp_ID", "first_name");
            ViewBag.getrolelistname = list;

            var getemp = db.Employees.Where(x => !x.role_ID.Equals(7) && !x.role_ID.Equals(1));
            SelectList emplist = new SelectList(getemp, "emp_ID", "first_name");
            ViewBag.getempname = emplist;

            return View();
        }

        [HttpPost]
        public ActionResult CreateTeam(FormCollection data, TEAM t)
        {
            if (ModelState.IsValid)
            {
                var getrolelist = db.Employees.Where(x => x.role_ID.Equals(7));
                SelectList list = new SelectList(getrolelist, "emp_ID", "first_name");
                ViewBag.getrolelistname = list;

                var getemp = db.Employees.Where(x => !x.role_ID.Equals(7) && !x.role_ID.Equals(1));
                SelectList emplist = new SelectList(getemp, "emp_ID", "first_name");
                ViewBag.getempname = emplist;

                //Variables to store data from form
                var teamname = data["team_name"];
                var empID = data["emp_ID"];
                var managerID = data["manager_ID"];

                //splitting employee ID by ',' to get all the selected employees 
                string[] employees = empID.Split(',');

                //Check all team names in database
                var CheckTeamName = from e in db.TEAMs
                                    where e.team_name == teamname
                                    select e.team_name;

                //If team name does not exist proceed with insert operation else show error
                if(CheckTeamName.FirstOrDefault() == null)
                {
                    foreach (string s in employees)
                    {
                        TEAM team = new TEAM
                        {
                            team_name = teamname,
                            emp_ID = Convert.ToInt32(s),
                            manager_ID = Convert.ToInt32(managerID)
                        };

                        db.TEAMs.Add(team);
                        db.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(TEAM.team_name), "Team name already exists");
                    return View(t);
                }

                return RedirectToAction("ViewTeam");
            }
            else
            {
                return View(t);
            }
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