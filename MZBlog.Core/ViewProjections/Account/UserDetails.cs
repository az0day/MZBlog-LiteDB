using LiteDB;
using MZBlog.Core.Documents;

namespace MZBlog.Core.ViewProjections.Account
{
    public class GetUserDetails : IViewProjection<string, Author>
    {
        private readonly LiteDatabase _db;

        public GetUserDetails(LiteDatabase db)
        {
            _db = db;
        }

        public Author Project(string input)
        {
            return _db.GetCollection<Author>(DBTableNames.Authors).FindById(input);
        }
    }
}