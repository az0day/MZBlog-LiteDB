using LiteDB;
using MZBlog.Core.Documents;
using System;
using System.IO;

namespace MZBlog.Core.Tests
{
    public class LiteDBBackedTest : IDisposable
    {
        protected readonly LiteDatabase _db;

        public LiteDBBackedTest()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            _db = new LiteDatabase(dbPath + @"\blog.db");
            var authorCol = _db.GetCollection<Author>(DBTableNames.Authors);
            authorCol.EnsureIndex<string>(x => x.Id);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}