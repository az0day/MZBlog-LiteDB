using FluentAssertions;
using LiteDB;
using MZBlog.Core.Commands.Accounts;
using MZBlog.Core.Documents;
using Xunit;

namespace MZBlog.Core.Tests.Accounts
{
    public class LoginCommandTests : LiteDBBackedTest
    {
        [Fact]
        public void login_should_success_if_user_in_database()
        {
            using (var db = new LiteDatabase(DataBase.DbPath))
            {
                var authorCol = db.GetCollection<Author>(DBTableNames.Authors);
                var author = new Author
                    {
                        Email = "test@mz.yi",
                        HashedPassword = Hasher.GetMd5Hash("test")
                    };

                authorCol.Insert(author);
            }

            var loginCommandInvoker = new LoginCommandInvoker(DataBase);
            loginCommandInvoker.Execute(new LoginCommand
            {
                Email = "test@mz.yi",
                Password = "test"
            }).Success.Should().BeTrue();
        }

        [Fact]
        public void login_should_fail_if_invalid_password_provided()
        {
            var documtnt = new Author
            {
                Email = "username@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("psw1")
            };

            using (var db = new LiteDatabase(DataBase.DbPath))
            {
                var authorCol = db.GetCollection<Author>(DBTableNames.Authors);
                authorCol.Insert(documtnt);
            }

            var loginCommandInvoker = new LoginCommandInvoker(DataBase);
            var command = new LoginCommand
                {
                    Email = "username@mz.yi",
                    Password = "psw2"
                };

            loginCommandInvoker.Execute(command).Success.Should().BeFalse();
        }
    }
}