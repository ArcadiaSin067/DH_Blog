using DH_Blog.Helpers;
using DH_Blog.Models;
using PagedList;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DH_Blog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await EmailHelper.ComposeEmailAsync(model);
                    return View("Index", "Home");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
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

    }
}