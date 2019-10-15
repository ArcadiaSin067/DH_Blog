using DH_Blog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DH_Blog.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var emailFrom = $"{model.FromEmail}<{ConfigurationManager.AppSettings["emailto"]}>";
                    var email = new MailMessage(emailFrom,
                                ConfigurationManager.AppSettings["emailto"])
                    {
                    Subject = model.Subject,
                    Body = model.Body,
                    IsBodyHtml = true
                    };
                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);
                    return View(new EmailModel());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }


        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var publishedBlogPosts = db.BlogPosts.Where(b => b.Published).ToList();
            return View(publishedBlogPosts);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();
            return View(model);
        }
    }
}