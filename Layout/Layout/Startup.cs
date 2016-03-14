using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Layout.Startup))]
namespace Layout
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
