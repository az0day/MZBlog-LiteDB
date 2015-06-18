using LiteDB;
using MZBlog.Core.Documents;

namespace MZBlog.Core.Commands.Posts
{
    public class DeleteCommentCommand
    {
        public string CommentId { get; set; }
    }

    public class DeleteCommentCommandInvoker : ICommandInvoker<DeleteCommentCommand, CommandResult>
    {
        private readonly Config _dbConfig;

        public DeleteCommentCommandInvoker(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public CommandResult Execute(DeleteCommentCommand command)
        {
            using (var _db = new LiteDatabase(_dbConfig.DbPath))
            {
                var blogCommentCol = _db.GetCollection<BlogComment>(DBTableNames.BlogComments);
                blogCommentCol.Delete(command.CommentId);
                return CommandResult.SuccessResult;
            }
        }
    }
}