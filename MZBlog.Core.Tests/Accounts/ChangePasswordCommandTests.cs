using FluentAssertions;
using LiteDB;
using MZBlog.Core.Commands.Accounts;
using MZBlog.Core.Documents;
using Xunit;

namespace MZBlog.Core.Tests.Accounts
{
    public class ChangePasswordCommandTests : LiteDBBackedTest
    {
        private const string AUTHOR_ID = "mzyi";

        [Fact]
        public void change_password_fail_if_old_password_does_not_match()
        {
            var author = new Author
            {
                Id = AUTHOR_ID,
                Email = "test@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("mzblog")
            };

            using (var db = new LiteDatabase(DataBase.DbPath))
            {
                var authorCol = db.GetCollection<Author>(DBTableNames.Authors);
                authorCol.Insert(author);
            }

            new ChangePasswordCommandInvoker(DataBase)
               .Execute(new ChangePasswordCommand
               {
                   AuthorId = author.Id,
                   OldPassword = "wrong psw",
                   NewPassword = "pswtest",
                   NewPasswordConfirm = "pswtest"
               })
               .Success.Should().BeFalse();
        }

        [Fact]
        public void change_password()
        {
            var author = new Author
            {
                Email = "test@mz.yi",
                HashedPassword = Hasher.GetMd5Hash("mzblog")
            };

            using (var db = new LiteDatabase(DataBase.DbPath))
            {
                var authors = db.GetCollection<Author>(DBTableNames.Authors);
                authors.Insert(author);
            }

            new ChangePasswordCommandInvoker(DataBase)
                 .Execute(new ChangePasswordCommand
                 {
                     AuthorId = author.Id,
                     OldPassword = "mzblog",
                     NewPassword = "pswtest",
                     NewPasswordConfirm = "pswtest"
                 })
                 .Success.Should().BeTrue();

            using (var db = new LiteDatabase(DataBase.DbPath))
            {
                var authors = db.GetCollection<Author>(DBTableNames.Authors);
                authors.FindById(author.Id).HashedPassword.Should().BeEquivalentTo(Hasher.GetMd5Hash("pswtest"));
            }
        }

        ~ChangePasswordCommandTests()
        {
            using (var db = new LiteDatabase(DataBase.DbPath))
            {
                var authorCol = db.GetCollection<Author>(DBTableNames.Authors);
                authorCol.Delete(AUTHOR_ID);
            }
        }
    }
}