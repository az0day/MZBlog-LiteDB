using LiteDB;
using MZBlog.Core.Documents;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class AllStatisticsViewModel
    {
        public long PostsCount { get; set; }

        public long CommentsCount { get; set; }

        public int TagsCount { get; set; }
    }

    public class AllStatisticsBindingModel
    {
        public AllStatisticsBindingModel()
        {
            TagThreshold = 1;
        }

        public int TagThreshold { get; set; }
    }

    public class AllStatisticsViewProjection : IViewProjection<AllStatisticsBindingModel, AllStatisticsViewModel>
    {
        private readonly Config _dbConfig;

        public AllStatisticsViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public AllStatisticsViewModel Project(AllStatisticsBindingModel input)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var blogPostCol = db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
                var postCount = blogPostCol.Count();
                if (postCount == 0)
                    return new AllStatisticsViewModel();
                var blogCommentCol = db.GetCollection<BlogComment>(DBTableNames.BlogComments);
                var stat = new AllStatisticsViewModel
                {
                    PostsCount = postCount,
                    CommentsCount = blogCommentCol.Count()
                };
                var tagCol = db.GetCollection<Tag>(DBTableNames.Tags);
                stat.TagsCount = tagCol.Count();

                return stat;
            }
        }
    }
}