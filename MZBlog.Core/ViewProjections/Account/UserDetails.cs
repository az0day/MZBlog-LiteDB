using LiteDB;
using MZBlog.Core.Documents;

namespace MZBlog.Core.ViewProjections.Account
{
    public class GetUserDetails : IViewProjection<string, Author>
    {
        private readonly Config _dbConfig;

        public GetUserDetails(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public Author Project(string input)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var authorCol = db.GetCollection<Author>(DBTableNames.Authors);
                return authorCol.FindById(input);
            }
        }
    }
}