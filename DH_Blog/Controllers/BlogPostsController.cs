using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DH_Blog.Helpers;
using DH_Blog.Models;
using PagedList;


namespace DH_Blog.Controllers
{
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);
            int pageSize = 5; // the number of posts you want to display per page
            int pageNumber = (page ?? 1);
            return View(blogList.ToPagedList(pageNumber, pageSize));

            //int pageSize = 5; // display three blog posts at a time on this page
            //int pageNumber = (page ?? 1);
            //var myblogposts = db.BlogPosts;
            //return View(db.BlogPosts.OrderByDescending(b => b.Created).ToPagedList(pageNumber,pageSize));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UnPubIndex(int? page,string searchStr)
        {
            var blogList = IndexSearch(searchStr);
            int pageSize = 5; // the number of posts you want to display per page
            int pageNumber = (page ?? 1);

            var pubPosts = db.BlogPosts.Where(b => !b.Published).ToList();
            return View("Index", pubPosts.ToPagedList(pageNumber, pageSize));
        }

        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) ||
                p.BlogPostBody.Contains(searchStr) ||
                p.Comments.Any(c => c.CommentBody.Contains(searchStr) ||
                c.Author.FirstName.Contains(searchStr) ||
                c.Author.LastName.Contains(searchStr) ||
                c.Author.DisplayName.Contains(searchStr) ||
                c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }
            return result.OrderByDescending(p => p.Created);
        }


        // GET: BlogPosts/Details/5
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Abstract,BlogPostBody,Published")] BlogPost blogPost, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (string.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                if(db.BlogPosts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }
                if (ImageUploadValidator.IsWebFriendlyImage(Image))
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    Image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.ImagePath = "/Uploads/" + fileName;
                }

                blogPost.Slug = Slug;
                blogPost.Created = DateTime.Now;
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        //GET: BlogPosts/Edit/5
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, Title, Abstract, BlogPostBody,Published,Created,Slug, ImagePath")] BlogPost blogPost, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var newSlug = StringUtilities.URLFriendly(blogPost.Title);
                if(newSlug != blogPost.Slug)
                {
                    if (string.IsNullOrWhiteSpace(newSlug))
                    {
                        ModelState.AddModelError("Title", "Invalid title");
                        return View(blogPost);
                    }
                    if (db.BlogPosts.Any(p => p.Slug == newSlug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique");
                        return View(blogPost);
                    }
                    blogPost.Slug = newSlug;
                }
                if (ImageUploadValidator.IsWebFriendlyImage(Image))
                {
                    var fileName = Path.GetFileName(Image.FileName);
                    Image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.ImagePath = "/Uploads/" + fileName;
                }

                blogPost.Updated = DateTime.Now;
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
