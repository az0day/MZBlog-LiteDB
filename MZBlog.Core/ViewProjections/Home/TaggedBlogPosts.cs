using LiteDB;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;
using MZBlog.Core.Extensions;

namespace MZBlog.Core.ViewProjections.Home
{
    public class TaggedBlogPostsViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }

        public string Tag { get; set; }
    }

    public class TaggedBlogPostsBindingModel
    {
        public string Tag { get; set; }
    }

    public class TaggedBlogPostsViewProjection : IViewProjection<TaggedBlogPostsBindingModel, TaggedBlogPostsViewModel>
    {
        private readonly Config _dbConfig;

        public TaggedBlogPostsViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public TaggedBlogPostsViewModel Project(TaggedBlogPostsBindingModel input)
        {
            if (string.IsNullOrWhiteSpace(input.Tag))
            {
                return null;
            }

            var slug = input.Tag.ToSlug();
            if (string.IsNullOrWhiteSpace(slug))
            {
                return null;
            }

            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var blogPostCol = db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
                var posts = (
                    from p in blogPostCol.FindAll()
                    where p.IsPublished && p.Tags.Contains(slug)
                    orderby p.PubDate descending
                    select p
                ).ToList();

                if (posts.Count == 0)
                {
                    return null;
                }

                var tags = db.GetCollection<Tag>(DBTableNames.Tags);
                var text = "Unknown";
                var tag = tags.FindOne(x => x.Slug.Equals(slug));
                if (tag != null)
                {
                    text = tag.Name;
                }

                return new TaggedBlogPostsViewModel
                {
                    Posts = posts,
                    Tag = text
                };
            }
        }
    }
}