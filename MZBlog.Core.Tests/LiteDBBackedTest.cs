using LiteDB;
using MZBlog.Core.Documents;
using System;
using System.IO;

namespace MZBlog.Core.Tests
{
    public class LiteDBBackedTest : IDisposable
    {
        protected readonly Config _dbConfig;

        public LiteDBBackedTest()
        {
            var dbDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            _dbConfig = new Config { DbPath = Path.Combine(dbDir, "blog.db") };
            using (var _db = new LiteDatabase(_dbConfig.DbPath))
            {
                var authorCol = _db.GetCollection<Author>(DBTableNames.Authors);
                authorCol.EnsureIndex<string>(x => x.Id);
            }
        }

        public void Dispose()
        {
            //_dbConfig = null;
        }
    }
}