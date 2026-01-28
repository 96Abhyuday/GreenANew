using GreenANew.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GreenANew.Controllers
{
    public class NGOController : BaseController
    {
        GreenANewEntities db = new GreenANewEntities();
        // GET: NGO
        public ActionResult Index()
        {
            int id = Convert.ToInt32(Session["UserId"]);
            var vm = new NGODash
            {
                AssignedTo = db.tasks.Count(t => t.AssignedTo == id && t.Status == "Assigned"),
                InProgress = db.tasks.Count(t => t.AssignedTo == id && t.Status == "InProgress"),
                Completed = db.tasks.Count(t => t.AssignedTo == id && t.Status == "Completed")
            };
            return View(vm);
        }
        [HttpGet]
        public ActionResult AssignedTasks()
        {
            int workerId = Convert.ToInt32(Session["UserId"]);
            var row = db.tasks.Include("Issue").Where(t => t.AssignedTo == workerId).ToList();
             return View(row);
        }
        [HttpGet]
        public ActionResult TaskDetails( int Id)
        {
            var model = db.tasks.Include("Issue").FirstOrDefault(x => x.TaskId == Id);
            if(model == null)
            {
                return HttpNotFound();
            }
            if (model.Status == "Assigned")
            {
                model.Status = "InProgress";
                model.Issue.Status = "InProgress";
                db.SaveChanges();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult TaskDetails(int TaskId, HttpPostedFileBase CompletionImage)
        {
            try
            {
                var task = db.tasks.Include("Issue").FirstOrDefault(t => t.TaskId == TaskId);
                if (task == null)
                {
                    return HttpNotFound();
                }


                if (CompletionImage != null && CompletionImage.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(CompletionImage.FileName);
                    string path = Server.MapPath("~/Content/TaskImages/" + fileName);
                    CompletionImage.SaveAs(path);

                    task.CompletionImage = fileName;
                }

                task.Status = "Completed";
                task.CompletedAt = DateTime.Now;
                task.Issue.Status = "Completed";

                db.SaveChanges();

                TempData["Success"] = "Task completed successfully!";
                return RedirectToAction("TaskDetails", "NGO",new { Id = TaskId });
            }

            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("TaskDetails", "NGO");
            }
        }
    }
}