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
    [RequireHttps]
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
                    TempData["sent"] = "Success";
                    return RedirectToAction("Contact");
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
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearchClass.IndexSearch(searchStr);
            int pageSize = 3; // the number of posts you want to display per page
            int pageNumber = (page ?? 1);
            return View(blogList.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

    }
}