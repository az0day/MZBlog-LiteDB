using LiteDB;
using MZBlog.Core.Documents;
using System;
using System.IO;

namespace MZBlog.Core.Tests
{
    public class LiteDBBackedTest : IDisposable
    {
        protected readonly Config DataBase;

        public LiteDBBackedTest()
        {
            var dbDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            DataBase = new Config
            {
                DbPath = Path.Combine(dbDir, "blog.db")
            };

            using (var db = new LiteDatabase(DataBase.DbPath))
            {
                var authorCol = db.GetCollection<Author>(DBTableNames.Authors);
                authorCol.EnsureIndex(x => x.Id);
            }
        }

        public void Dispose()
        {
            //_dbConfig = null;
        }
    }
}