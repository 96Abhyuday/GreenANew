using GreenANew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GreenANew.Controllers
{
    public class AdminController : BaseController
    {
        GreenANewEntities db = new GreenANewEntities();
        // GET: Admin
        public ActionResult Index()
        {
            var vm = new NGODash
            {
                AssignedTo = db.Issues.Count(t =>  t.Status == "Pending"),
                Completed = db.Issues.Count(t => t.Status == "Completed"),
                InProgress = db.Issues.Count(t => t.Status == "Assigned"),
                UserCount = db.Users.Count((t => t.Role == "Citizen"))
            };
            return View(vm);
        }
        [HttpGet]
        public ActionResult IssueManagement()
        {
            var model = db.Issues.Include("User").ToList();
            return View(model);
        }
        [HttpGet]
        public ActionResult TaskManagement()
        {
            var model = db.tasks.Include("Issue").Include("User").ToList();
            return View(model);
        }
        [HttpGet]
        public ActionResult RejectIssue(int issueId)
        {
            try
            {
                var issue = db.Issues.FirstOrDefault(i => i.IssueId == issueId);
                if (issue == null)
                {
                    TempData["Error"] = "Issue not found!";
                    return RedirectToAction("IssueManagement");
                }

                if (issue.Status == "Completed")
                {
                    TempData["Error"] = "Completed issue cannot be rejected!";
                    return RedirectToAction("IssueManagement");
                }

                issue.Status = "Rejected"; 
                db.SaveChanges();

                TempData["Success"] = "Issue rejected successfully!";
                return RedirectToAction("IssueManagement");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException != null? ex.InnerException.Message : ex.Message;
                return RedirectToAction("IssueManagement");
            }
        }
        [HttpGet]
        public ActionResult ReopenIssue(int issueId)
        {
            try
            {
                var issue = db.Issues.FirstOrDefault(i => i.IssueId == issueId);
                if (issue == null)
                {
                    TempData["Error"] = "Issue not found!";
                    return RedirectToAction("IssueManagement");
                }

                if (issue.Status != "Rejected")
                {
                    TempData["Error"] = "Only rejected issues can be reopened!";
                    return RedirectToAction("IssueManagement");
                }

                issue.Status = "Pending";
                db.SaveChanges();

                TempData["Success"] = "Issue reopened successfully!";
                return RedirectToAction("IssueManagement");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException != null
                    ? ex.InnerException.Message
                    : ex.Message;

                return RedirectToAction("IssueManagement");
            }
        }

        [HttpGet]
        public ActionResult CreateEvent()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateEvent(PlantationEvent model)
        {
            try
            {
                ModelState.Remove("CreatedBy");
                ModelState.Remove("Status");

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                PlantationEvent row = new PlantationEvent
                {
                    Title = model.Title,
                    Description = model.Description,
                    Location = model.Location,
                    EventDate = model.EventDate,
                    CreatedBy = Convert.ToInt32(Session["UserId"]),
                    Status = "Scheduled"
                };
                db.PlantationEvents.Add(row);
                db.SaveChanges();
                TempData["Success"] = "Event Created Successfully!";
                return RedirectToAction("EventManagement");
            }
            catch (Exception ex)
            {
                //TempData["Error"] = "Event Creation Failed!";
                TempData["Error"] = ex.InnerException.InnerException.Message;
                return View(model);
            }
        }
        [HttpGet]
        public ActionResult EventManagement()
        {
            return View(db.PlantationEvents.ToList());
        }
        [HttpGet]
        public ActionResult UserManagement()
        {
            var user = db.Users.ToList();
            return View(user);
        }
    }
}