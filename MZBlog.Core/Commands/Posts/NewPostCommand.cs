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
                foreach (var tag in tags)
                {
                    var slug = tag.ToSlug();
                    var tagEntry = _db.GetCollection<Tag>(DBTableNames.Tags).FindById(slug);
                    if (tagEntry == null)
                    {
                        tagEntry = new Tag
                        {
                            Slug = slug,
                            Name = tag,
                            PostCount = 1
                        };
                        _db.GetCollection<Tag>(DBTableNames.Tags).Insert(tagEntry);
                    }
                    else
                    {
                        tagEntry.PostCount++;
                        _db.GetCollection<Tag>(DBTableNames.Tags).Update(tagEntry);
                    }
                }
            }
            else
                post.Tags = new string[] { };

            var result = _db.GetCollection<BlogPost>(DBTableNames.BlogPosts).Insert(post);

            return CommandResult.SuccessResult;
        }
    }
}