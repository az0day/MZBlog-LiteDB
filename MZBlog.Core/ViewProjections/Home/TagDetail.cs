using LiteDB;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class TagDetailViewProjection : IViewProjection<string, Tag>
    {
        private readonly Config _dbConfig;

        public TagDetailViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public Tag Project(string input)
        {
            using (var _db = new LiteDatabase(_dbConfig.DbPath))
            {
                var tagCol = _db.GetCollection<Tag>(DBTableNames.Tags);
                var tag = tagCol.FindOne(x => x.Name == input);
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
            using (var _db = new LiteDatabase(_dbConfig.DbPath))
            {
                var tagCol = _db.GetCollection<Tag>(DBTableNames.Tags);
                var tags = from slug in input
                           select tagCol.FindOne(x => x.Name == slug);
                return tags;
            }
        }
    }
}