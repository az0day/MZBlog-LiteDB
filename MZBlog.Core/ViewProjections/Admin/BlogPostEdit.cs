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
        private readonly Config _dbConfig;

        public BlogPostEditViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public BlogPostEditViewModel Project(BlogPostEditBindingModel input)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var posts = db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
                var post = posts.FindById(input.PostId);
                return new BlogPostEditViewModel { BlogPost = post };
            }
        }
    }
}