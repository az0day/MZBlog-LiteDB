using LiteDB;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class TagDetailViewProjection : IViewProjection<string, Tag>
    {
        private readonly LiteDatabase _db;

        public TagDetailViewProjection(LiteDatabase db)
        {
            _db = db;
        }

        public Tag Project(string input)
        {
            var tag = _db.GetCollection<Tag>(DBTableNames.Tags).FindOne(x => x.Name == input);
            return tag;
        }
    }

    public class TagsViewProjection : IViewProjection<IEnumerable<string>, IEnumerable<Tag>>
    {
        private readonly LiteDatabase _db;

        public TagsViewProjection(LiteDatabase db)
        {
            _db = db;
        }

        public IEnumerable<Tag> Project(IEnumerable<string> input)
        {
            var tags = from slug in input
                       select _db.GetCollection<Tag>(DBTableNames.Tags).FindOne(x => x.Name == slug);
            return tags;
        }
    }
}