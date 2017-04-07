using MZBlog.Core;
using Nancy.TinyIoc;

namespace MZBlog.Web.Features
{
    public class CommandInvokerFactory : ICommandInvokerFactory
    {
        private readonly TinyIoCContainer _container;

        public CommandInvokerFactory(TinyIoCContainer containtr)
        {
            _container = containtr;
        }

        public TOut Handle<TIn, TOut>(TIn input)
        {
            WriteLogs("LogsExecute", string.Format("TIn: {0}", Serialize(input)));

            var loadtr = _container.Resolve<ICommandInvoker<TIn, TOut>>();
            var ret = loadtr.Execute(input);

            WriteLogs("LogsExecute", string.Format("TOut: {0}", Serialize(ret)));
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