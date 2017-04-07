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
        private readonly Config _dbConfig;

        public RecentBlogPostSummaryViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public RecentBlogPostSummaryViewModel Project(RecentBlogPostSummaryBindingModel input)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var blogPostCol = db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
                var titles = (from p in blogPostCol.FindAll()
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
}