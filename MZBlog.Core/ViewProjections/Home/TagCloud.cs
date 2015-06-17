using LiteDB;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class TagCloudBindingModel
    {
        public int Threshold { get; set; }

        public int Take { get; set; }

        public TagCloudBindingModel()
        {
            Threshold = 1;
            Take = int.MaxValue;
        }
    }

    public class TagCloudViewModel
    {
        public IEnumerable<Tag> Tags { get; set; }
    }

    public class TagCloudViewProjection : IViewProjection<TagCloudBindingModel, TagCloudViewModel>
    {
        private readonly LiteDatabase _db;

        public TagCloudViewProjection(LiteDatabase db)
        {
            _db = db;
        }

        public TagCloudViewModel Project(TagCloudBindingModel input)
        {
            var tagCol = _db.GetCollection<Tag>(DBTableNames.Tags);
            var result = new Dictionary<Tag, int>();
            var tags = tagCol.FindAll().OrderByDescending(x => x.PostCount);

            return new TagCloudViewModel
            {
                Tags = tags
            };
        }
    }
}