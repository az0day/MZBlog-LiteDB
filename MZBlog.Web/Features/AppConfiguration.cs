using Nancy;
using System.Configuration;

namespace MZBlog.Web.Features
{
    public class AppConfiguration
    {
        private static readonly DynamicDictionary config = new DynamicDictionary();

        static AppConfiguration()
        {
            //initializing from configuraiton settings
            foreach (var appKey in ConfigurationManager.AppSettings.AllKeys)
            {
                config[appKey] = ConfigurationManager.AppSettings[appKey];
            }
        }

        public static dynamic Current
        {
            get { return config; }
        }

        private AppConfiguration()
        {
        }
    }
}