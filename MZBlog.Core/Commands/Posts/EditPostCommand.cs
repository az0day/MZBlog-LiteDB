using LiteDB;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System;
using System.Linq;

namespace MZBlog.Core.Commands.Posts
{
    public class EditPostCommand
    {
        public string PostId { get; set; }

        public string AuthorId { get; set; }

        public string Title { get; set; }

        public string TitleSlug { get; set; }

        public string MarkDown { get; set; }

        public string Tags { get; set; }

        public DateTime PubDate { get; set; }

        public bool Published { get; set; }
    }

    public class EditPostCommandInvoker : ICommandInvoker<EditPostCommand, CommandResult>
    {
        private readonly Config _dbConfig;

        public EditPostCommandInvoker(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public CommandResult Execute(EditPostCommand command)
        {
            using (var _db = new LiteDatabase(_dbConfig.DbPath))
            {
                var blogPostCol = _db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
                var post = blogPostCol.FindById(command.PostId);

                if (post == null)
                    throw new ApplicationException("Post with id: {0} was not found".FormatWith(command.PostId));
                if (post.Tags != null)
                {
                    var tagCol = _db.GetCollection<Tag>(DBTableNames.Tags);
                    foreach (var tag in post.Tags)
                    {
                        var slug = tag.ToSlug();
                        var tagEntry = tagCol.FindOne(x => x.Slug == slug);
                        if (tagEntry != null)
                        {
                            tagEntry.PostCount--;
                            tagCol.Update(tagEntry);
                        }
                    }
                }

                var markdown = new MarkdownSharp.Markdown();
                //TODO:应该验证TitleSlug是否是除了本文外唯一的

                post.MarkDown = command.MarkDown;
                post.Content = markdown.Transform(command.MarkDown);
                post.PubDate = command.PubDate.CloneToUtc();
                post.Status = command.Published ? PublishStatus.Published : PublishStatus.Draft;
                post.Title = command.Title;
                post.TitleSlug = command.TitleSlug.Trim().ToSlug();
                if (!command.Tags.IsNullOrWhitespace())
                {
                    var tags = command.Tags.Trim().Split(',').Select(s => s.Trim());
                    post.Tags = tags.Select(s => s.ToSlug()).ToArray();
                    var tagCol = _db.GetCollection<Tag>(DBTableNames.Tags);
                    foreach (var tag in tags)
                    {
                        var slug = tag.ToSlug();
                        var tagEntry = tagCol.FindOne(x => x.Slug == slug);
                        if (tagEntry == null)
                        {
                            tagEntry = new Tag
                            {
                                Slug = slug,
                                Name = tag,
                                PostCount = 1
                            };
                            tagCol.Insert(tagEntry);
                        }
                        else
                        {
                            tagEntry.PostCount++;
                            tagCol.Update(tagEntry);
                        }
                    }
                }
                else
                    post.Tags = new string[] { };
                _db.GetCollection<BlogPost>(DBTableNames.BlogPosts).Update(post);

                return CommandResult.SuccessResult;
            }
        }
    }
}