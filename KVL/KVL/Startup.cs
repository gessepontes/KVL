using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KVL.Startup))]
namespace KVL
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
          
        }
    }
}
