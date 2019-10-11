using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DH_Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public string AuthorId { get; set; }
        public DateTime Created { get; set; }

        [Display(Name="Comment")]
        public string CommentBody { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdateReason { get; set; }

        //virtual navigation section
        public virtual BlogPost BlogPost { get; set; }
        public virtual ApplicationUser Author { get; set; }

    }
}