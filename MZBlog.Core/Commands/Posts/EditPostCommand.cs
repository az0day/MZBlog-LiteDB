﻿using System.Text;
using Kiwi.Markdown;
using Kiwi.Markdown.ContentProviders;
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
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var blogPostCol = db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
                var post = blogPostCol.FindById(command.PostId);

                if (post == null)
                {
                    throw new ApplicationException("Post with id: {0} was not found".FormatWith(command.PostId));
                }

                if (post.Tags != null)
                {
                    var tagCol = db.GetCollection<Tag>(DBTableNames.Tags);
                    foreach (var tag in post.Tags)
                    {
                        var slug = tag.ToSlug();
                        var tagEntry = tagCol.FindOne(x => x.Slug.Equals(slug));
                        if (tagEntry != null)
                        {
                            tagEntry.PostCount--;
                            tagCol.Update(tagEntry);
                        }
                    }
                }

                var contentProvider = new FileContentProvider(null, Encoding.UTF8);
                var parser = new MarkdownService(contentProvider);
                var content = parser.ToHtml(command.MarkDown);
                //TODO:应该验证TitleSlug是否是除了本文外唯一的

                post.MarkDown = command.MarkDown;
                post.Content = content;
                post.PubDate = command.PubDate.CloneToUtc();
                post.Status = command.Published ? PublishStatus.Published : PublishStatus.Draft;
                post.Title = command.Title;
                post.TitleSlug = command.TitleSlug.Trim().ToSlug();

                if (!command.Tags.IsNullOrWhitespace())
                {
                    var tags = command.Tags.AsTags();
                    post.Tags = tags.Keys.ToArray();
                    
                    var tagCol = db.GetCollection<Tag>(DBTableNames.Tags);
                    foreach (var kv in tags)
                    {
                        var slug = kv.Key;
                        var tag = kv.Value;

                        var entry = tagCol.FindOne(x => x.Slug.Equals(slug));
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
                }
                else
                {
                    post.Tags = new string[] { };
                }

                db.GetCollection<BlogPost>(DBTableNames.BlogPosts).Update(post);
                return CommandResult.SuccessResult;
            }
        }
    }
}