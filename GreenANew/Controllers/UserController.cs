using GreenANew.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GreenANew.Controllers
{
    public class UserController : BaseController
    {
        GreenANewEntities db = new GreenANewEntities();
        // GET: User
        public ActionResult Index()
        {
            int id = Convert.ToInt32(Session["UserId"]);
            var vm = new CitizenDashVM
            {
                TotalIssues = db.Issues.Count(i => i.UserId == id),
                PendingIssues = db.Issues.Count(i => i.UserId == id && i.Status == "Pending"),
                ResolvedIssues = db.Issues.Count(i => i.UserId == id && i.Status == "Completed"),
                EventsJoined = db.EventParticipants.Count(e => e.UserId == id)
            };
            return View(vm);
        }
        [HttpGet]
        public ActionResult Report()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Report(Issue model)
        {
            try
            {
                string fileName = null;
                HttpPostedFileBase file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Content/IssueImg/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string fullPath = Path.Combine(folderPath, fileName);
                    file.SaveAs(fullPath);
                }

                Issue row = new Issue
                {
                    UserId = Convert.ToInt32(Session["UserId"]), 
                    IssueType = model.IssueType,
                    Location = model.Location,
                    Discription = model.Discription,
                    Image = fileName,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                db.Issues.Add(row); 
                db.SaveChanges();

                TempData["Success"] = "Issue reported successfully!";
                return RedirectToAction("Issues");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Issues()
        {
            int Id = Convert.ToInt32(Session["UserId"]);
            var model = db.Issues.FirstOrDefault(m => m.UserId == Id);
            return View(db.Issues.ToList());
        }
        [HttpGet]
        public ActionResult IssueDetails(int Id)
        {
            Issue model = db.Issues.Include("tasks").FirstOrDefault(m => m.IssueId == Id);
            if(model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult PlantationEvents()
        {
            var events = db.PlantationEvents.Where(x => x.Status == "Scheduled").ToList();
            return View(events);
        }
        [HttpGet]
        public ActionResult MyProfile()
        {
            int Id = Convert.ToInt32(Session["UserId"]);
            User model = db.Users.FirstOrDefault(m => m.UserId == Id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
       
    }
}