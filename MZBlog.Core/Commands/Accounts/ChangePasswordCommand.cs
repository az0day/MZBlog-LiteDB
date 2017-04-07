using LiteDB;
using MZBlog.Core.Documents;

namespace MZBlog.Core.Commands.Accounts
{
    public class ChangePasswordCommand
    {
        public string AuthorId { get; set; }

        public string NewPassword { get; set; }

        public string NewPasswordConfirm { get; set; }

        public string OldPassword { get; set; }
    }

    public class ChangePasswordCommandInvoker : ICommandInvoker<ChangePasswordCommand, CommandResult>
    {
        private readonly Config _dbConfig;

        public ChangePasswordCommandInvoker(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public CommandResult Execute(ChangePasswordCommand command)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var authorCol = db.GetCollection<Author>(DBTableNames.Authors);
                var author = authorCol.FindById(command.AuthorId);
                if (Hasher.GetMd5Hash(command.OldPassword) != author.HashedPassword)
                {
                    return new CommandResult("旧密码不正确!");
                }

                author.HashedPassword = Hasher.GetMd5Hash(command.NewPassword);
                authorCol.Update(author);
                return CommandResult.SuccessResult;
            }
        }
    }
}