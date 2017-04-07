using LiteDB;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class RecentBlogPostsViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }

        public int Page { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPrevPage
        {
            get
            {
                return Page > 1;
            }
        }
    }

    public class RecentBlogPostsBindingModel
    {
        public RecentBlogPostsBindingModel()
        {
            Page = 1;
            Take = 20;
        }

        public int Page { get; set; }

        public int Take { get; set; }
    }

    public class RecentBlogPostViewProjection : IViewProjection<RecentBlogPostsBindingModel, RecentBlogPostsViewModel>
    {
        private readonly Config _dbConfig;

        public RecentBlogPostViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public RecentBlogPostsViewModel Project(RecentBlogPostsBindingModel input)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var skip = (input.Page - 1) * input.Take;
                var blogPostCol = db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
                var posts = (from p in blogPostCol.FindAll()
                             where p.IsPublished
                             orderby p.PubDate descending
                             select p)
                             .Skip(skip)
                             .Take(input.Take + 1)
                             .ToList()
                             .AsReadOnly();

                var pagedPosts = posts.Take(input.Take).ToList();
                var hasNextPage = posts.Count > input.Take;

                return new RecentBlogPostsViewModel
                {
                    Posts = pagedPosts,
                    Page = input.Page,
                    HasNextPage = hasNextPage
                };
            }
        }
    }
}