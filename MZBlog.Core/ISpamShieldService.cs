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
        private readonly LiteDatabase _db;

        public SpamShieldService(LiteDatabase db)
        {
            _db = db;
        }

        public string CreateTick(string key)
        {
            var tick = ObjectId.NewObjectId().ToString();
            var spamHash = new SpamHash
            {
                Id = tick,
                PostKey = key,
                CreatedTime = DateTime.UtcNow
            };
            _db.GetCollection<SpamHash>(DBTableNames.SpamHashes).Insert(spamHash);
            return tick;
        }

        public string GenerateHash(string tick)
        {
            var nonhash = string.Empty;
            if (tick.IsNullOrWhitespace())
                return nonhash;

            var spamHash = _db.GetCollection<SpamHash>(DBTableNames.SpamHashes).FindOne(x => x.PostKey == tick);

            if (spamHash == null || spamHash.Pass || !spamHash.Hash.IsNullOrWhitespace())
                return nonhash;

            spamHash.Hash = new Random().NextDouble().ToString();
            _db.GetCollection<SpamHash>(DBTableNames.SpamHashes).Update(spamHash);

            return spamHash.Hash;
        }

        public bool IsSpam(SpamShield command)
        {
            if (command.Tick.IsNullOrWhitespace() || command.Hash.IsNullOrWhitespace())
                return true;

            var spamHash = _db.GetCollection<SpamHash>(DBTableNames.SpamHashes).FindOne(x => x.PostKey == command.Tick);

            if (spamHash == null || spamHash.Pass || spamHash.Hash != command.Hash)
                return true;

            spamHash.Pass = true;
            _db.GetCollection<SpamHash>(DBTableNames.SpamHashes).Update(spamHash);
            return false;
        }
    }
}