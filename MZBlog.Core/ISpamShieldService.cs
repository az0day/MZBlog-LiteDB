using LiteDB;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace MZBlog.Core
{
    public class SpamShield
    {
        [Required]
        public string Tick { get; set; }

        [Required]
        public string Hash { get; set; }
    }

    public interface ISpamShieldService
    {
        string CreateTick(string key);

        string GenerateHash(string tick);

        bool IsSpam(SpamShield command);
    }

    public class SpamShieldService : ISpamShieldService
    {
        private readonly Config _dbConfig;

        public SpamShieldService(Config dbConfig)
        {
            //var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "litedb");
            //if (!Directory.Exists(dbPath))
            //{
            //    Directory.CreateDirectory(dbPath);
            //}

            //dbConfig = Path.Combine(dbPath, "blog.db");
            _dbConfig = dbConfig;
        }

        public string CreateTick(string key)
        {
            using (var _db = new LiteDatabase(_dbConfig.DbPath))
            {
                var tick = ObjectId.NewObjectId().ToString();
                var spamHash = new SpamHash
                {
                    Id = tick,
                    PostKey = key,
                    CreatedTime = DateTime.UtcNow
                };
                var spamHashCol = _db.GetCollection<SpamHash>(DBTableNames.SpamHashes);
                spamHashCol.Insert(spamHash);
                return tick;
            }
        }

        public string GenerateHash(string tick)
        {
            var nonhash = string.Empty;
            if (tick.IsNullOrWhitespace())
                return nonhash;

            using (var _db = new LiteDatabase(_dbConfig.DbPath))
            {
                var spamHashCol = _db.GetCollection<SpamHash>(DBTableNames.SpamHashes);
                var spamHash = spamHashCol.FindOne(x => x.PostKey == tick);

                if (spamHash == null || spamHash.Pass || !spamHash.Hash.IsNullOrWhitespace())
                    return nonhash;

                spamHash.Hash = new Random().NextDouble().ToString();
                spamHashCol.Update(spamHash);

                return spamHash.Hash;
            }
        }

        public bool IsSpam(SpamShield command)
        {
            using (var _db = new LiteDatabase(_dbConfig.DbPath))
            {
                if (command.Tick.IsNullOrWhitespace() || command.Hash.IsNullOrWhitespace())
                    return true;

                var spamHashCol = _db.GetCollection<SpamHash>(DBTableNames.SpamHashes);
                var spamHash = spamHashCol.FindOne(x => x.PostKey == command.Tick);

                if (spamHash == null || spamHash.Pass || spamHash.Hash != command.Hash)
                    return true;

                spamHash.Pass = true;
                spamHashCol.Update(spamHash);
                return false;
            }
        }
    }
}