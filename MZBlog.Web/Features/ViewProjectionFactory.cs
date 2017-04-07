using MZBlog.Core;
using Nancy.TinyIoc;

namespace MZBlog.Web.Features
{
    public class ViewProjectionFactory : IViewProjectionFactory
    {
        private readonly TinyIoCContainer _container;

        public ViewProjectionFactory(TinyIoCContainer containtr)
        {
            _container = containtr;
        }

        public TOut Get<TIn, TOut>(TIn input)
        {
            WriteLogs("LogsView", string.Format("TIn: {0}", Serialize(input)));

            var loadtr = _container.Resolve<IViewProjection<TIn, TOut>>();
            var ret = loadtr.Project(input);

            WriteLogs("LogsView", string.Format("TOut: {0}", Serialize(ret)));
            return ret;
        }

        private static void WriteLogs(string title, string content)
        {
            BaseDefined.Helpers.LogsHelper.Append(title, content);
        }

        private static string Serialize(object data)
        {
            return BaseDefined.Helpers.BinaryHelper.SerializeToString((data));
        }
    }
}