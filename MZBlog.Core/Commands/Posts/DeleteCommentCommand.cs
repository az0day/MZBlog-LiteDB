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
        private readonly LiteDatabase _db;

        public DeleteCommentCommandInvoker(LiteDatabase db)
        {
            _db = db;
        }

        public CommandResult Execute(DeleteCommentCommand command)
        {
            var blogCommentCol = _db.GetCollection<BlogComment>(DBTableNames.BlogComments);
            blogCommentCol.Delete(command.CommentId);
            return CommandResult.SuccessResult;
        }
    }
}