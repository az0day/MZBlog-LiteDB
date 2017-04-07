﻿using LiteDB;
using MZBlog.Core.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MZBlog.Core.ViewProjections.Home
{
    public class TagCloudBindingModel
    {
        public int Threshold { get; set; }

        public int Take { get; set; }

        public TagCloudBindingModel()
        {
            Threshold = 1;
            Take = int.MaxValue;
        }
    }

    public class TagCloudViewModel
    {
        public IEnumerable<Tag> Tags { get; set; }
    }

    public class TagCloudViewProjection : IViewProjection<TagCloudBindingModel, TagCloudViewModel>
    {
        private readonly Config _dbConfig;

        public TagCloudViewProjection(Config dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public TagCloudViewModel Project(TagCloudBindingModel input)
        {
            using (var db = new LiteDatabase(_dbConfig.DbPath))
            {
                var tagCol = db.GetCollection<Tag>(DBTableNames.Tags);
                var tags = tagCol.FindAll().OrderByDescending(x => x.PostCount).ToArray();

                return new TagCloudViewModel
                {
                    Tags = tags
                };
            }
        }
    }
}