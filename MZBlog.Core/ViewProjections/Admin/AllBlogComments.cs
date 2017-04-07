using LiteDB;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class AllBlogCommentsBindingModel
    {
        public AllBlogCommentsBindingModel()
        {
            Page = 1;
            Take = 20;
        }

        public int Page { get; set; }

        public int Take { get; set; }
    }

    public class AllBlogCommentsViewModel
    {
        public IEnumerable<BlogComment> Comments { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPrevPage
        {
            get
            {
                return Page > 1;
            }
        }

        public int Page { get; set; }
    }

    public class BlogCommentsViewProjection : IViewProjection<AllBlogCommentsBindingModel, AllBlogCommentsViewModel>
    {
        private readonly Config _dbConfig;

        public BlogCommentsViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public AllBlogCommentsViewModel Project(AllBlogCommentsBindingModel input)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var skip = (input.Page - 1) * input.Take;
                var blogCommentCol = db.GetCollection<BlogComment>(DBTableNames.BlogComments);
                var comments = blogCommentCol
                    .Find(Query.All(), skip, input.Take + 1)
                    .OrderByDescending(b => b.CreatedTime)
                    .ToList().AsReadOnly();

                var pagedComments = comments.Take(input.Take);
                var hasNextPage = comments.Count > input.Take;

                return new AllBlogCommentsViewModel
                {
                    Comments = pagedComments,
                    Page = input.Page,
                    HasNextPage = hasNextPage
                };
            }
        }
    }
}