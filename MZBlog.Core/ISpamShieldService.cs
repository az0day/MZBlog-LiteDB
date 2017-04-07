using LiteDB;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

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

        public string CreateTick(string slug)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var tick = ObjectId.NewObjectId().ToString();
                var entry = new SpamHash
                {
                    Id = tick,
                    PostKey = slug,
                    CreatedTime = DateTime.UtcNow
                };

                var cols = db.GetCollection<SpamHash>(DBTableNames.SpamHashes);
                cols.Insert(entry);
                return tick;
            }
        }

        public string GenerateHash(string tick)
        {
            var nonhash = string.Empty;
            if (tick.IsNullOrWhitespace())
            {
                return nonhash;
            }

            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var cols = db.GetCollection<SpamHash>(DBTableNames.SpamHashes);
                var entry = cols.FindOne(x => x.Id == tick);

                if (entry == null || entry.Pass || !entry.Hash.IsNullOrWhitespace())
                {
                    return nonhash;
                }

                entry.Hash = ObjectId.NewObjectId().ToString();
                cols.Update(entry);
                
                return entry.Hash;
            }
        }

        public bool IsSpam(SpamShield command)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                if (command.Tick.IsNullOrWhitespace() || command.Hash.IsNullOrWhitespace())
                {
                    return true;
                }

                var cols = db.GetCollection<SpamHash>(DBTableNames.SpamHashes);
                var entry = cols.FindOne(x => x.Id == command.Tick);

                if (entry == null || entry.Pass || entry.Hash != command.Hash)
                {
                    return true;
                }

                entry.Pass = true;
                cols.Update(entry);
                return false;
            }
        }
    }
}