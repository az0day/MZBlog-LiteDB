﻿namespace MZBlog.Core.Documents
{
    public class Tag
    {
        public string Id { get; set; }

        public string Slug { get; set; }

        public string Name { get; set; }

        public int PostCount { get; set; }
    }
}