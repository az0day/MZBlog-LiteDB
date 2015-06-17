using LiteDB;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System;
using System.Collections.Generic;
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
        private readonly LiteDatabase _db;

        public NewPostCommandInvoker(LiteDatabase db)
        {
            _db = db;
        }

        public CommandResult Execute(NewPostCommand command)
        {
            var markdown = new MarkdownSharp.Markdown();
            //TODO:应该验证TitleSlug是否唯一
            var post = new BlogPost
                           {
                               Id = ObjectId.NewObjectId(),
                               AuthorEmail = command.Author.Email,
                               AuthorDisplayName = command.Author.DisplayName,
                               MarkDown = command.MarkDown,
                               Content = markdown.Transform(command.MarkDown),
                               PubDate = command.PubDate.CloneToUtc(),
                               Status = command.Published ? PublishStatus.Published : PublishStatus.Draft,
                               Title = command.Title,
                               TitleSlug = command.TitleSlug.IsNullOrWhitespace() ? command.Title.Trim().ToSlug() : command.TitleSlug.Trim().ToSlug(),
                               DateUTC = DateTime.UtcNow
                           };
            if (!command.Tags.IsNullOrWhitespace())
            {
                var tags = command.Tags.Trim().Split(',').Select(s => s.Trim());
                post.Tags = tags.Select(s => s.ToSlug()).ToArray();
                var tagCol = _db.GetCollection<Tag>(DBTableNames.Tags);
                foreach (var tag in tags)
                {
                    var slug = tag.ToSlug();
                    var tagEntry = tagCol.FindById(slug);
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
            var blogPostCol = _db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
            var result = blogPostCol.Insert(post);

            return CommandResult.SuccessResult;
        }
    }
}