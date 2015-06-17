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
        private readonly LiteDatabase _db;

        public AllStatisticsViewProjection(LiteDatabase db)
        {
            _db = db;
        }

        public AllStatisticsViewModel Project(AllStatisticsBindingModel input)
        {
            var postCount = _db.GetCollection<BlogPost>(DBTableNames.BlogPosts).Count();
            if (postCount == 0)
                return new AllStatisticsViewModel();

            var stat = new AllStatisticsViewModel
            {
                PostsCount = postCount,
                CommentsCount = _db.GetCollection<BlogComment>(DBTableNames.BlogComments).Count()
            };

            stat.TagsCount = _db.GetCollection<Tag>(DBTableNames.Tags).Count();

            return stat;
        }
    }
}