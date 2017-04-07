using System.Text;
using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;
using LiteDB;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System;
using System.Linq;

namespace MZBlog.Core.Commands.Posts
{
    public class NewPostCommand
    {
        public Author Author { get; set; }

        public string Title { get; set; }

        public string TitleSlug { get; set; }

        public string MarkDown { get; set; }

        public string Tags { get; set; }

        public DateTime PubDate { get; set; }

        public bool Published { get; set; }
    }

    public class NewPostCommandInvoker : ICommandInvoker<NewPostCommand, CommandResult>
    {
        private readonly Config _dbConfig;

        public NewPostCommandInvoker(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public CommandResult Execute(NewPostCommand command)
        {
            //TODO:应该验证TitleSlug是否唯一
            var contentProvider = new FileContentProvider(null, Encoding.UTF8);
            var parser = new MarkdownService(contentProvider);
            var content = parser.ToHtml(command.MarkDown);

            var post = new BlogPost
            {
                Id = ObjectId.NewObjectId(),
                AuthorEmail = command.Author.Email,
                AuthorDisplayName = command.Author.DisplayName,
                MarkDown = command.MarkDown,
                Content = content,
                PubDate = command.PubDate.CloneToUtc(),
                Status = command.Published ? PublishStatus.Published : PublishStatus.Draft,
                Title = command.Title,
                TitleSlug = command.TitleSlug.IsNullOrWhitespace() ? command.Title.Trim().ToSlug() : command.TitleSlug.Trim().ToSlug(),
                DateUTC = DateTime.UtcNow
            };

            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                if (!command.Tags.IsNullOrWhitespace())
                {
                    var tags = command.Tags.AsTags();
                    post.Tags = tags.Keys.ToArray();

                    var tagCol = db.GetCollection<Tag>(DBTableNames.Tags);
                    foreach (var kv in tags)
                    {
                        var slug = kv.Key;
                        var tag = kv.Value;

                        if (string.IsNullOrWhiteSpace(tag))
                        {
                            continue;
                        }

                        var entry = tagCol.FindOne(t => t.Slug.Equals(slug));
                        if (entry != null)
                        {
                            entry.PostCount++;
                            tagCol.Update(entry);
                        }
                        else
                        {
                            entry = new Tag
                            {
                                Id = ObjectId.NewObjectId(),
                                Slug = slug,
                                Name = tag,
                                PostCount = 1
                            };
                            tagCol.Insert(entry);
                        }
                    }

                    tagCol.EnsureIndex(t => t.PostCount);
                }
                else
                {
                    post.Tags = new string[] { };
                }

                var blogPostCol = db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
                blogPostCol.Insert(post);

                return CommandResult.SuccessResult;
            }
        }
    }
}