using LiteDB;
using MZBlog.Core.Documents;

namespace MZBlog.Core.ViewProjections.Admin
{
    public class AuthorProfileViewModel
    {
        public string DisplayName { get; set; }

        public string Email { get; set; }
    }

    public class AuthorProfileViewProjection : IViewProjection<string, AuthorProfileViewModel>
    {
        private readonly Config _dbConfig;

        public AuthorProfileViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public AuthorProfileViewModel Project(string input)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var authorCol = db.GetCollection<Author>(DBTableNames.Authors);
                var author = authorCol.FindById(input);
                if (author == null)
                    return null;
                return new AuthorProfileViewModel
                {
                    DisplayName = author.DisplayName,
                    Email = author.Email
                };
            }
        }
    }
}