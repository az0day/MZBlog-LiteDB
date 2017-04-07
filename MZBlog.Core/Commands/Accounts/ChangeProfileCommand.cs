using LiteDB;
using MZBlog.Core.Documents;

namespace MZBlog.Core.Commands.Accounts
{
    public class ChangeProfileCommand
    {
        public string AuthorId { get; set; }

        public string NewEmail { get; set; }

        public string NewDisplayName { get; set; }
    }

    public class ChangeProfileCommandInvoker : ICommandInvoker<ChangeProfileCommand, CommandResult>
    {
        private readonly Config _dbConfig;

        public ChangeProfileCommandInvoker(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public CommandResult Execute(ChangeProfileCommand command)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var authorCol = db.GetCollection<Author>(DBTableNames.Authors);
                var author = authorCol.FindById(command.AuthorId);
                if (author == null)
                    return new CommandResult("用户信息不存在");
                author.DisplayName = command.NewDisplayName;
                author.Email = command.NewEmail;

                authorCol.Update(author);
                return CommandResult.SuccessResult;
            }
        }
    }
}