using LiteDB;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class RecentBlogPostSummaryViewModel
    {
        public IEnumerable<BlogPostSummary> BlogPostsSummaries { get; set; }
    }

    public class BlogPostSummary
    {
        public string Title { get; set; }

        public string Link { get; set; }
    }

    public class RecentBlogPostSummaryBindingModel
    {
        public int Page { get; set; }
    }

    public class RecentBlogPostSummaryViewProjection : IViewProjection<RecentBlogPostSummaryBindingModel, RecentBlogPostSummaryViewModel>
    {
        private readonly LiteDatabase _db;

        public RecentBlogPostSummaryViewProjection(LiteDatabase db)
        {
            _db = db;
        }

        public RecentBlogPostSummaryViewModel Project(RecentBlogPostSummaryBindingModel input)
        {
            var titles = (from p in _db.GetCollection<BlogPost>(DBTableNames.BlogPosts).FindAll()
                          where p.IsPublished
                          orderby p.PubDate descending
                          select new BlogPostSummary
                          {
                              Title = p.Title,
                              Link = p.GetLink()
                          }
                          )
                .Take(input.Page)
                .ToList()
                .AsReadOnly();

            return new RecentBlogPostSummaryViewModel { BlogPostsSummaries = titles };
        }
    }
}