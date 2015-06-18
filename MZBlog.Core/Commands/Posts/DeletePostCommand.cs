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
            using (var _db = new LiteDatabase(_dbConfig.DbPath))
            {
                var blogCommentCol = _db.GetCollection<BlogComment>(DBTableNames.BlogComments);
                blogCommentCol.Delete(x => x.PostId == command.PostId);
                var blogPostCol = _db.GetCollection<BlogPost>(DBTableNames.BlogPosts);
                blogPostCol.Delete(x => x.Id == command.PostId);

                return CommandResult.SuccessResult;
            }
        }
    }
}