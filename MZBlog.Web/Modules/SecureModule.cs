using MZBlog.Core;
using MZBlog.Core.Documents;
using MZBlog.Core.Extensions;
using MZBlog.Web.Security;
using Nancy;
using Nancy.Extensions;

namespace MZBlog.Web.Modules
{
    public class SecureModule : BaseNancyModule
    {
        protected readonly IViewProjectionFactory _viewProjectionFactory;

        public SecureModule(IViewProjectionFactory viewProjectionFactory)
        {
            _viewProjectionFactory = viewProjectionFactory;

            Before += SetContextUserFromAuthenticationCookie;
            Before += SetCurrentUserToViewBag;
            Before += SetCurrentUserToParamsForBindingPurposes;
        }

        private Response SetCurrentUserToParamsForBindingPurposes(NancyContext ctx)
        {
            ctx.Parameters.AuthorId = ctx.CurrentUser.UserName;
            return null;
        }

        private Response SetCurrentUserToViewBag(NancyContext ctx)
        {
            var author = _viewProjectionFactory.Get<string, Author>(ctx.CurrentUser.UserName);
            if (author == null)
                return ctx.GetRedirect("/mz-login?returnUrl=" + Request.Url.Path).WithCookie(FormsAuthentication.CreateLogoutCookie());

            ViewBag.CurrentUser = author;
            return null;
        }

        public Author CurrentUser
        {
            get { return (Author)ViewBag.CurrentUser.Value; }
        }

        private Response SetContextUserFromAuthenticationCookie(NancyContext ctx)
        {
            var username = FormsAuthentication.GetAuthUsernameFromCookie(ctx);

            if (username.IsNullOrWhitespace())
            {
                var url = string.Format("/mz-login?returnUrl={0}", Request.Url.Path);
                var cookie = FormsAuthentication.CreateLogoutCookie();
                return ctx.GetRedirect(url).WithCookie(cookie);
            }

            ctx.CurrentUser = new BlogUserIdentity(username, new [] { "admin" });

            return null;
        }
    }
}