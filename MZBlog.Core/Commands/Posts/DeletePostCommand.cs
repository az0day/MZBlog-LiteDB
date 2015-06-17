using LiteDB;
using MZBlog.Core.Documents;
using System.Linq;

namespace MZBlog.Core.Commands.Posts
{
    public class DeletePostCommand
    {
        public string PostId { get; set; }
    }

    public class DeletePostCommandInvoker : ICommandInvoker<DeletePostCommand, CommandResult>
    {
        private readonly LiteDatabase _db;

        public DeletePostCommandInvoker(LiteDatabase db)
        {
            _db = db;
        }

        public CommandResult Execute(DeletePostCommand command)
        {
            _db.GetCollection<BlogComment>(DBTableNames.BlogComments).Delete(x => x.PostId == command.PostId);

            _db.GetCollection<BlogPost>(DBTableNames.BlogPosts).Delete(x => x.Id == command.PostId);

            return CommandResult.SuccessResult;
        }
    }
}