using LiteDB;
using MZBlog.Core.Documents;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class BlogPostEditBindingModel
    {
        public string PostId { get; set; }
    }

    public class BlogPostEditViewModel
    {
        public BlogPost BlogPost { get; set; }
    }

    public class BlogPostEditViewProjection : IViewProjection<BlogPostEditBindingModel, BlogPostEditViewModel>
    {
        private readonly LiteDatabase _db;

        public BlogPostEditViewProjection(LiteDatabase db)
        {
            _db = db;
        }

        public BlogPostEditViewModel Project(BlogPostEditBindingModel input)
        {
            var blogPostCol = _db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
            var post = blogPostCol.FindById(input.PostId);
            return new BlogPostEditViewModel { BlogPost = post };
        }
    }
}