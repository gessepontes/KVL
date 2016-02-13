using KVL.Context;
using KVL.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace KVL
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public object UserService { get; private set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;

            ModelBinders.Binders.Add(
                typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(
                typeof(decimal?), new DecimalModelBinder());
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                ModeloDados db = new ModeloDados();

                var ticket = FormsAuthentication.Decrypt(authCookie.Value);

                FormsIdentity formsIdentity = new FormsIdentity(ticket);

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(formsIdentity);

                var user = db.Login.Where(p => p.sLogin == ticket.Name).ToList();

                foreach (var role in user)
                {
                    claimsIdentity.AddClaim(
                        new Claim(ClaimTypes.Role, role.IDPerfil.ToString()));
                }

                if (ticket.Name.ToUpper() == "ADMIN") {
                    claimsIdentity.AddClaim(
    new Claim(ClaimTypes.Role, "Administrador"));
                }

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.Current.User = claimsPrincipal;
            }
        }
    }
}
