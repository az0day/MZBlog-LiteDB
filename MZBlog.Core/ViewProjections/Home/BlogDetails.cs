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
        private readonly Config _dbConfig;

        public BlogPostDetailsViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public BlogPostDetailsViewModel Project(BlogPostDetailsBindingModel input)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var blogPostCol = db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
                var post = blogPostCol.FindOne(x => x.TitleSlug == input.Permalink);
                if (post == null)
                    return null;
                post.ViewCount++;
                blogPostCol.Update(post);
                var blogCommentCol = db.GetCollection<BlogComment>(DBTableNames.BlogComments);
                var comments = blogCommentCol.Find(x => x.PostId == post.Id)
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
}