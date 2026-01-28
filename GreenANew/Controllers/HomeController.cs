using GreenANew.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GreenANew.Controllers
{
    public class HomeController : Controller
    {
        GreenANewEntities db = new GreenANewEntities();
        [HttpGet]

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User mem)
        {
            try
            {
                var member = db.Users.FirstOrDefault(m => m.Email == mem.Email && m.Password == mem.Password && m.Role == mem.Role && m.IsActive == true);
                if(member != null)
                {
                    Session["UserId"] = member.UserId;
                    Session["UserName"] = member.Name;
                    Session["UserRole"] = member.Role;

                    if (mem.Role == "Worker")
                    {
                        return RedirectToAction("Index", "NGO");
                    }
                    if (mem.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "User");
                    }
                }
                else
                {
                    TempData["Problem"] = "Email Or Password Is Not Match";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Login Failed";
                return View(mem);
            }
            return View();
        }
        [HttpGet]

        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Registration(User model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existingMail = db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingMail != null)
            {
                TempData["Email"] = "Email already registered!";
                return View(model);
            }
            try
            {
                User user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role,
                    Address = model.Address,
                    City = model.City,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                db.Users.Add(user);
                db.SaveChanges();
                Response.Write("<script>alert('Registration Successful! Please login.');window.location.href='/Home/Login'</script>");
            }
            catch(Exception)
            {
                TempData["Error"] = "Registration Unsuccessful!";
                return View(model);
            }
            return View();
        }

        [HttpGet]
        public ActionResult AdminEditUserRole(int Id)
        {
            User model = db.Users.FirstOrDefault(x => x.UserId == Id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AdminEditUserRole(User model)
        {
            try
            {
                User member = db.Users.FirstOrDefault(m => m.UserId == model.UserId);
                if (member != null)
                {
                    member.Role = model.Role;
                    db.SaveChanges();
                    Response.Write("<script>alert('Updation Successful!');window.location.href='/Admin/UserManagement'</script>");
                }
                else
                {
                    TempData["problem"] = "Updatation Faild!";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Update Faild!";
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult AdminEditEvent(int id)
        {
            var events = db.PlantationEvents.FirstOrDefault(x => x.EventId == id);
            return View(events);
        }
        [HttpPost]
        public ActionResult AdminEditEvent(PlantationEvent events)
        {
            try
            {
                PlantationEvent mem = db.PlantationEvents.FirstOrDefault(x => x.EventId == events.EventId);
                if(mem != null)
                {
                    mem.EventDate = events.EventDate;
                    db.SaveChanges();
                    return RedirectToAction("EventManagement", "Admin");
                }
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Updation Failed";
                return RedirectToAction("AdminEditEvent", "Home");
            }
            return View();
        }
        [HttpGet]
        public ActionResult AdminEventMembers(int id)
        {
            var vm = new EventMembersVM
            {
                Event = db.PlantationEvents.FirstOrDefault(e => e.EventId == id),
                Participants = db.EventParticipants.Include("User").Where(p => p.EventId == id).ToList()
            };

            return View(vm);
        }
        [HttpGet]
        public ActionResult Join(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Home");

            int userId = Convert.ToInt32(Session["UserId"]);

            bool alreadyJoined = db.EventParticipants
                .Any(x => x.EventId == id && x.UserId == userId);

            if (alreadyJoined)
            {
                TempData["Error"] = "Already joined this event";
                return RedirectToAction("Index","User");
            }

            EventParticipant ep = new EventParticipant
            {
                EventId = id,
                UserId = userId,
                JoinedAt = DateTime.Now
            };

            db.EventParticipants.Add(ep);
            db.SaveChanges();

            TempData["Success"] = "Event joined successfully!";
            return RedirectToAction("Index","User");
        }


        [HttpGet]
        public ActionResult EditProfile()
        {
            int id = Convert.ToInt32(Session["UserId"]);
            User model = db.Users.FirstOrDefault(x => x.UserId == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult EditProfile(User model)
        {
            try
            {
                User member = db.Users.FirstOrDefault(m => m.UserId == model.UserId);
                if (member != null)
                {
                    member.Name = model.Name;
                    member.Email = model.Email;
                    member.Password = model.Password;
                    db.SaveChanges();
                    Response.Write("<script>alert('Updation Successful!');window.location.href='/User/MyProfile'</script>");
                }
                else
                {
                    TempData["problem"] = "Updatation Faild!";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Update Faild!";
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult AssignTask(int issueId)
        {
            ViewBag.IssueId = issueId;

            ViewBag.Workers = db.Users
                .Where(u => u.Role == "Worker" && u.IsActive)
                .ToList();

            return View();
        }

        [HttpPost]
        public ActionResult AssignTask(task model)
        {
            try
            {
                // 1️⃣ Basic validation
                if (model.IssueId <= 0 || model.AssignedTo <= 0)
                {
                    TempData["Error"] = "Invalid Issue or Worker selected!";
                    ViewBag.IssueId = model.IssueId;
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.TaskType))
                {
                    TempData["Error"] = "Task Type is required!";
                    ViewBag.IssueId = model.IssueId;
                    return View(model);
                }

                // 2️⃣ Check Issue exists
                var issue = db.Issues.FirstOrDefault(i => i.IssueId == model.IssueId);
                if (issue == null)
                {
                    TempData["Error"] = "Issue not found!";
                    ViewBag.IssueId = model.IssueId;
                    return View(model);
                }

                // 3️⃣ Create task
                task newTask = new task
                {
                    IssueId = model.IssueId,
                    AssignedTo = model.AssignedTo,
                    TaskType = model.TaskType,
                    Deadline = model.Deadline,
                    Status = "Assigned",  
                    CompletedAt = null
                };

                db.tasks.Add(newTask);

                // 4️⃣ Update Issue status (ONLY allowed values)
                issue.Status = "Assigned";   // ✅ must match CHECK constraint

                // 5️⃣ Save changes
                db.SaveChanges();

                TempData["Success"] = "Task Assigned Successfully!";
                return RedirectToAction("IssueManagement","Admin");
            }
            catch (Exception ex)
            {
                ViewBag.IssueId=model.IssueId;

                TempData["Error"] = ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;

                return View(model);
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }

    }
}