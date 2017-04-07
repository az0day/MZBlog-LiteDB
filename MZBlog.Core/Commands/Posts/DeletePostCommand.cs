using LiteDB;
using MZBlog.Core.Documents;

namespace MZBlog.Core.Commands.Posts
{
    public class DeletePostCommand
    {
        public string PostId { get; set; }
    }

    public class DeletePostCommandInvoker : ICommandInvoker<DeletePostCommand, CommandResult>
    {
        private readonly Config _dbConfig;

        public DeletePostCommandInvoker(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public CommandResult Execute(DeletePostCommand command)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var posts = db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
                var post = posts.FindById(command.PostId);
                if (post != null)
                {
                    var tags = db.GetCollection<Tag>(DBTableNames.Tags);
                    foreach (var slug in post.Tags)
                    {
                        string slug1 = slug;
                        var tagEntry = tags.FindOne(x => x.Slug.Equals(slug1));
                        if (tagEntry != null)
                        {
                            tagEntry.PostCount--;
                            tags.Update(tagEntry);
                        }
                    }
                }

                // remove
                posts.Delete(x => x.Id == command.PostId);

                var comments = db.GetCollection<BlogComment>(DBTableNames.BlogComments);
                comments.Delete(x => x.PostId == command.PostId);

                return CommandResult.SuccessResult;
            }
        }
    }
}