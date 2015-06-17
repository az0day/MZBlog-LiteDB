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
        private readonly LiteDatabase _db;

        public AuthorProfileViewProjection(LiteDatabase db)
        {
            _db = db;
        }

        public AuthorProfileViewModel Project(string input)
        {
            var author = _db.GetCollection<Author>(DBTableNames.Authors).FindById(input);
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