using DH_Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DH_Blog.Helpers
{
    public class IndexSearchClass
    {
        public static IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            var db = new ApplicationDbContext();
            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.Where(b => b.Published);
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
                result = db.BlogPosts.Where(b => b.Published);
            }
            return result.OrderByDescending(p => p.Created);
        }

    }
}