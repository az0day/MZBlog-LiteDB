using LiteDB;
using MZBlog.Core.Documents;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class BlogPostDetailsBindingModel
    {
        public string Permalink { get; set; }
    }

    public class BlogPostDetailsViewModel
    {
        public BlogPost BlogPost { get; set; }

        public BlogComment[] Comments { get; set; }
    }

    public class BlogPostDetailsViewProjection : IViewProjection<BlogPostDetailsBindingModel, BlogPostDetailsViewModel>
    {
        private readonly LiteDatabase _db;

        public BlogPostDetailsViewProjection(LiteDatabase db)
        {
            _db = db;
        }

        public BlogPostDetailsViewModel Project(BlogPostDetailsBindingModel input)
        {
            var post = _db.GetCollection<BlogPost>(DBTableNames.BlogPosts).FindOne(x => x.TitleSlug == input.Permalink);
            if (post == null)
                return null;
            post.ViewCount++;
            _db.GetCollection<BlogPost>(DBTableNames.BlogPosts).Update(post);

            var comments = _db.GetCollection<BlogComment>(DBTableNames.BlogComments).Find(x => x.PostId == post.Id)
                                    .OrderBy(o => o.CreatedTime)
                                    .ToArray();

            return new BlogPostDetailsViewModel
            {
                BlogPost = post,
                Comments = comments
            };
        }
    }
}