using LiteDB;
using MZBlog.Core.Documents;
using System.Collections.Generic;

namespace MZBlog.Core.ViewProjections.Home
{
    public class TagDetailViewProjection : IViewProjection<string, Tag>
    {
        private readonly Config _dbConfig;

        public TagDetailViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public Tag Project(string slug)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var tagCol = db.GetCollection<Tag>(DBTableNames.Tags);
                var tag = tagCol.FindOne(x => x.Slug.Equals(slug));
                return tag;
            }
        }
    }

    public class TagsViewProjection : IViewProjection<IEnumerable<string>, IEnumerable<Tag>>
    {
        private readonly Config _dbConfig;

        public TagsViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public IEnumerable<Tag> Project(IEnumerable<string> input)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var tagCol = db.GetCollection<Tag>(DBTableNames.Tags);
                var tags = new List<Tag>();
                foreach (var slug in input)
                {
                    var slug1 = slug;
                    var tag = tagCol.FindOne(x => x.Slug.Equals(slug1));
                    if (tag != null)
                    {
                        tags.Add(tag);
                    }
                }

                return tags;
            }
        }
    }
}