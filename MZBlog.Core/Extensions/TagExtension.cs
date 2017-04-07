using MZBlog.Core.Documents;

namespace MZBlog.Core.Extensions
{
    public static class TagExtension
    {
        private static IViewProjectionFactory factory;

        public static void SetupViewProjectionFactory(IViewProjectionFactory fac)
        {
            factory = fac;
        }

        public static Tag AsTag(this string tag)
        {
            var slug = tag.ToSlug();
            return factory.Get<string, Tag>(slug);
        }
    }
}