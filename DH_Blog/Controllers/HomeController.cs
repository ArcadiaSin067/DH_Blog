using DH_Blog.Models;
using PagedList;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
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
                    Body = $"Incoming Email from your Blog <hr /> { model.Body }",
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
        public ActionResult Index(int? page)
        {
            int pageSize = 3; // display three blog posts at a time on this page
            int pageNumber = (page ?? 1);
            var publishedBlogPosts = db.BlogPosts.Where(b => b.Published).ToList();
            return View(db.BlogPosts.OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));
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